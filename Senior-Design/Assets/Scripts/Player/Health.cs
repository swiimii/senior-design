using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Health : NetworkBehaviour, IDamageable
{
    public const int MAXHEALTH = 3;

    public GameObject healthBarObject;
    public float maxBarLength;
    public string mostRecentAttacker;
    public NetworkVariable<bool> recentlyDamaged;

    [SerializeField]
    NetworkVariable<int> health = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone);

    private void Start()
    {
        maxBarLength = healthBarObject.transform.localScale.x;
        health.OnValueChanged += ResolveHealthChange;
        if (IsServer)
        {
            health.Value = MAXHEALTH;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void HealServerRpc(int value)
    {
        health.Value = Mathf.Min(health.Value + value, MAXHEALTH);
    }

    [ServerRpc(RequireOwnership=false)]
    public void DamageServerRpc(int value, string source)
    {
        if (!recentlyDamaged.Value)
        {
            mostRecentAttacker = source;
            health.Value -= value;
        }
        if (health.Value > 0)
        {
            recentlyDamaged.Value = true;
            StartCoroutine(Knockback(Vector3.zero));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(int value, string source, Vector3 knockbackDirection)
    {
        StartCoroutine(Knockback(knockbackDirection));

        if (!recentlyDamaged.Value)
        {
            recentlyDamaged.Value = true;
            mostRecentAttacker = source;
            health.Value -= value;
        }


    }

    public void ResolveHealthChange(int oldstate, int newstate)
    {
        var ht = healthBarObject.transform;
        ht.localScale = new Vector3(Mathf.Min(Mathf.Max((float)newstate / MAXHEALTH * maxBarLength, 0), MAXHEALTH), ht.localScale.y, ht.localScale.z);

        if (IsServer && newstate <= 0)
        {
            ResolveHealthDepletionClientRpc();
            health.Value = MAXHEALTH;
        }
    }


    [ClientRpc]
    public void ResolveHealthDepletionClientRpc()
    {
        var spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
        transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, transform.position.z);
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
    
    // server only
    public IEnumerator Knockback(Vector3 direction, float knockbackMagnitude = 7)
    {
        TogglePlayerControlClientRpc(false);
        var kbVector = direction.normalized * knockbackMagnitude;
        KnockbackClientRpc(kbVector);

        // stun for 1 second, then return control to player
        yield return new WaitForSeconds(1);
        recentlyDamaged.Value = false;
        TogglePlayerControlClientRpc(true);
    }

    [ClientRpc]
    public void KnockbackClientRpc(Vector3 kbVector)
    {
        GetComponent<Rigidbody2D>().velocity = kbVector;
    }

    [ClientRpc]
    public void TogglePlayerControlClientRpc(bool playerCanControl)
    {
        if(IsLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = playerCanControl;
        }
        GetComponent<SpriteRenderer>().color = playerCanControl ? Color.white : Color.red;
    }
}
