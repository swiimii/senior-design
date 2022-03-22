using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkAnimationSync : NetworkBehaviour
{
    public NetworkVariable<float> horizontal, vertical;
    private void Start()
    {
        if(IsServer)
        {
            // horizontal = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone);
        }
    }
    // Update is called once per frame
    void Update()
    {
        var anim = GetComponent<Animator>();
        if (IsLocalPlayer)
        {
            var horiz = anim.GetFloat("HorizontalMovement");
            var vert = anim.GetFloat("VerticalMovement"); ;
            UpdateAnimationServerRpc(horiz, vert);
        }
        else
        {
            anim.SetFloat("HorizontalMovement", horizontal.Value);
            anim.SetFloat("VerticalMovement", vertical.Value);
        }
    }

    [ServerRpc]
    void UpdateAnimationServerRpc(float horiz, float vert)
    {
        var anim = GetComponent<Animator>();
        horizontal.Value = horiz;
        vertical.Value = vert;
    }

}
