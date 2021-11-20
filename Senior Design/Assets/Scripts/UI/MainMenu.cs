using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MLAPI;
using MLAPI.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public InputField nameInput;

    private void Awake()
    {
        nameInput.text = PlayerPrefs.GetString("Name");
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
            NetworkSceneManager.SwitchScene(lobbySceneName);
        }
    }

    public void JoinGame()
    {
        var inputtedName = nameInput.text;
        if (!inputtedName.Equals(""))
        {
            PlayerPrefs.SetString("Name", inputtedName);
            PlayerPrefs.Save();
            NetworkManager.Singleton.StartClient();
        }
    }
}
