using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class MainMenu : MonoBehaviour
{
    public InputField nameInput, addressInput;

    private void Awake()
    {
        nameInput.text = PlayerPrefs.GetString("Name");
        addressInput.text = PlayerPrefs.GetString("LastAddress");
    }

    public void StartGame()
    {
        var inputtedName = nameInput.text;
        if (!inputtedName.Equals(""))
        {
            PlayerPrefs.SetString("Name", inputtedName);
            PlayerPrefs.Save();
            var lobbySceneName = "Lobby";
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);
        }
    }

    public void JoinGame()
    {
        var inputtedName = nameInput.text;
        var targetAddress = addressInput.text;
        PlayerPrefs.SetString("Name", inputtedName);
        PlayerPrefs.SetString("LastAddress", targetAddress);
        PlayerPrefs.Save();

        if (!inputtedName.Equals("") && !targetAddress.Equals(""))
        {
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = targetAddress;
            try
            {
                NetworkManager.Singleton.StartClient();
                StartCoroutine(WaitForConnection());
            }
            catch
            {
                NetworkManager.Singleton.Shutdown();
            }
            
        }
    }

    IEnumerator WaitForConnection()
    {
        // Time before running the coroutine logic.
        yield return new WaitForSeconds(2);

        if (NetworkManager.Singleton.IsConnectedClient == false)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
