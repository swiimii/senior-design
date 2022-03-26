using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Postgame : NetworkBehaviour
{

    public void ReturnToLobbyButton()
    {
        if (IsHost)
        {
            ReturnToLobbyServerRpc();
        }
    }

    public void LeaveButton()
    {
        if (IsHost)
        {
            LeaveServerRpc();
        }
        else
        {
            LeaveLobby();
        }
    }

    [ServerRpc]
    private void ReturnToLobbyServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void CloseLobby()
    {
        if (IsHost)
        {
            KickClientRpc();
        }
    }

    [ServerRpc]
    private void LeaveServerRpc()
    {
        KickClientRpc();
    }

    [ClientRpc]
    private void KickClientRpc()
    {
        LeaveLobby();
    }

    private void LeaveLobby()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
