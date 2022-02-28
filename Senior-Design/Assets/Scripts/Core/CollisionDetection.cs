using Unity.Netcode;
using UnityEngine;

public class CollisionDetection : NetworkBehaviour {
    [HideInInspector] public new Rigidbody2D rigidbody2D;
    private ContactFilter2D contactFilter;
    public ContactPoint2D? groundContact;
    public ContactPoint2D? ceilingContact;
    public ContactPoint2D? wallContact;
    public ContactPoint2D? anyContact;
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[16];

    [SerializeField] private LayerMask layerMask;

    private float maxWalkCos = 0.2f;

    public bool IsGrounded => groundContact.HasValue;
    public bool IsTouchingWall => wallContact.HasValue;
    public bool IsTouchingCeiling => ceilingContact.HasValue;
    public bool HittingSomething => anyContact.HasValue;

    public Vector2 Velocity => rigidbody2D.velocity;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layerMask);
    }
    private void FixedUpdate() {
        FindContacts();
    }

    public void ActivateTopDownCollision(Vector2 movementDirection) {
        var horizontalDirection = (int) Mathf.Sign(movementDirection.x);
        var verticalDirection = (int) Mathf.Sign(movementDirection.y);
        
        if (groundContact.HasValue) {
            var wallDirection = (int) Mathf.Sign(groundContact.Value.point.x - transform.position.x);
            if (verticalDirection == wallDirection)
                rigidbody2D.velocity = Vector2.zero;
        } else if (ceilingContact.HasValue) {
            var wallDirection = (int) Mathf.Sign(ceilingContact.Value.point.x - transform.position.x);
            if (verticalDirection == wallDirection)
                rigidbody2D.velocity = Vector2.zero;
        } else if (wallContact.HasValue) {
            var wallDirection = (int) Mathf.Sign(wallContact.Value.point.x - transform.position.x);
            if (horizontalDirection == wallDirection)
                rigidbody2D.velocity = Vector2.zero;
        }
    }

    private void FindContacts() {
        groundContact = null;
        ceilingContact = null;
        wallContact = null;
        anyContact = null;

        float groundProjection = maxWalkCos;
        float wallProjection = maxWalkCos;
        float ceilingProjection = -maxWalkCos;

        int numberOfContacts = rigidbody2D.GetContacts(contactFilter, contacts);
        for (var i = 0; i < numberOfContacts; i++) {
            var contact = contacts[i];
            float projection = Vector2.Dot(Vector2.up, contact.normal);

            if (projection > groundProjection) {
                groundContact = contact;
                anyContact = contact;
                groundProjection = projection;
            }
            else if (projection < ceilingProjection) {
                ceilingContact = contact;
                anyContact = contact;
                ceilingProjection = projection;
            }
            else if (projection <= wallProjection) {
                wallContact = contact;
                anyContact = contact;
                wallProjection = projection;
            }
        }
    }
}