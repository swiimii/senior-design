using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : NetworkSingleton<InputManager>
{
    [SerializeField] private InputProvider inputProvider;
    public InputProvider InputProvider => inputProvider;
    private void Awake() {
        inputProvider.EnableInput();
    }

    private void OnDisable() {
        inputProvider.DisableInput();
    }
}
