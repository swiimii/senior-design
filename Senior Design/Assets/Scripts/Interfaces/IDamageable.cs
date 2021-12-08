using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void DamageServerRpc(int value, string damageSource);
    public void ResolveHealthChange(int oldPlayerState, int newPlayerState);

    public void ResolveHealthDepletion();
}
