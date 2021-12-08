using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class Health : NetworkBehaviour, IDamageable
{
    public const int MAXHEALTH = 5;

    public GameObject healthBarObject;
    public float maxBarLength;
    public string mostRecentAttacker;

    [SerializeField]
    NetworkVariable<int> health = new NetworkVariable<int>();

    private void Start()
    {
        maxBarLength = healthBarObject.transform.localScale.x;
        if (IsServer)
        {
            health.Value = MAXHEALTH;
        }
        health.OnValueChanged += ResolveHealthChange;
        health.OnValueChanged += CheckIfDead;
    }

    [ServerRpc(RequireOwnership=false)]
    public void HealServerRpc(int value)
    {
        health.Value = Mathf.Min(health.Value + value, MAXHEALTH);
    }

    [ServerRpc(RequireOwnership=false)]
    public void DamageServerRpc(int value, string source)
    {
        mostRecentAttacker = source;
        health.Value -= value;
    }

    public void ResolveHealthChange(int oldstate, int newstate)
    {
        var ht = healthBarObject.transform;
        ht.localScale = new Vector3(Mathf.Min(Mathf.Max((float)health.Value / MAXHEALTH * maxBarLength, 0), MAXHEALTH), ht.localScale.y, ht.localScale.z);
    }

    public void CheckIfDead(int oldstate, int newstate)
    {
        if (newstate <= 0)
        {
            ResolveHealthDepletion();
        }
    }
    public void ResolveHealthDepletion()
    {

        transform.position = new Vector3(0, 0, 0);
        if (IsServer)
        {
            health.Value = MAXHEALTH;
        }
       
    }
}
