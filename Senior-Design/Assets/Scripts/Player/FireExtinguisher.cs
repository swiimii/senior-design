using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FireExtinguisher : NetworkBehaviour
{
    public GameObject particleObject;
    private Vector2 lastValidLookDirection = new Vector2(0, -1);
    private void Update()
    {
        var anim = GetComponentInParent<Animator>();
        Vector2 lookRotation = new Vector2(anim.GetFloat("HorizontalMovement"),
                                           anim.GetFloat("VerticalMovement")).normalized;
        // Vector3 diff = lookPosition
        if (Mathf.Abs(lookRotation.magnitude) > 0.1f)
        {
            particleObject.transform.rotation = Quaternion.LookRotation(lookRotation);
            
            lastValidLookDirection = lookRotation;
        }

        if (particleObject.activeInHierarchy)
        {
            var origin = transform.position;
            var distance = 4f;
            var hits = Physics2D.RaycastAll(origin, lastValidLookDirection, distance);
            Debug.DrawRay((Vector2)origin, lastValidLookDirection.normalized * distance);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject && hit.collider.gameObject.TryGetComponent<Hazard>(out var hazard))
                {
                    hazard.gameObject.transform.localScale = hazard.gameObject.transform.localScale - new Vector3(.4f, .4f, 0f) * Time.deltaTime;
                    if (hazard.gameObject.transform.localScale.x < .5f)
                    {
                        if (IsLocalPlayer)
                        {
                            print("Extinguished");
                            hazard.ExtinguishServerRpc();
                        }
                    }
                }
            }
        }
    }

    public void DoFireExtinguisher()
    {
        DoFireExtinguisherServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoFireExtinguisherServerRpc()
    {
        print("do");
        // particleObject.SetActive(true);
        DoFireExtinguisherClientRpc();
    }
    
    [ClientRpc]
    private void DoFireExtinguisherClientRpc()
    {
        particleObject.SetActive(true);
    }

    public void StopFireExtinguisher()
    {
        StopFireExtinguisherServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopFireExtinguisherServerRpc()
    {
        StopFireExtinguisherClientRpc();
    }

    [ClientRpc]
    private void StopFireExtinguisherClientRpc()
    {
        particleObject.SetActive(false);
    }

}
