using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EscapePod : NetworkInteractable
{
    [SerializeField] SpriteRenderer goButtonSprite;
    private bool isCompleting = false;

    private void OnEnable()
    {
        goButtonSprite.color = Color.green;
        
    }
    private void OnDisable()
    {
        goButtonSprite.color = Color.red;
    }

    [ServerRpc(RequireOwnership=false)]
    protected override void OnInteractServerRpc(ItemType item)
    {
        if (enabled && !isCompleting)
        {
            isCompleting = true;
            print("Game Complete!");
            CompleteGameServerRpc();
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void CompleteGameServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SuccessScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            p.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
