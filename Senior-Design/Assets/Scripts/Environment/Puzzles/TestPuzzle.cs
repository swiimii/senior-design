using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class TestPuzzle : IPuzzle
{
    private NetworkVariable<string> currentState;
    [SerializeField] string startingState, targetState;
    [SerializeField] InputField stateDisplay;

    private void Awake()
    {
        if (!NetworkManager.Singleton)
        {
            Debug.LogError("No Network Manager Detected");
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            currentState = new NetworkVariable<string>();
            currentState.Value = startingState;
        }
    }

    private void OnEnable()
    {
        currentState.OnValueChanged += UpdateDisplayedValue();
        print("enabled");
    }

    private void OnDisable()
    {
        currentState.OnValueChanged -= UpdateDisplayedValue();
        print("disabled");
    }

    private NetworkVariable<string>.OnValueChangedDelegate UpdateDisplayedValue()
    {
        stateDisplay.text = currentState.Value;
        print("updating value");
        return null;
    }

    public override bool CheckState()
    {
        return currentState.Value.CompareTo(targetState) == 0;
    }

    public void OnButtonClick()
    {
        var newVal = "" + Random.value;
        UpdateValueServerRpc("" + newVal);
        Debug.Log(currentState.Value);
    }

    [ServerRpc]
    public void UpdateValueServerRpc(string value)
    {
        currentState.Value = value;
    }
}
