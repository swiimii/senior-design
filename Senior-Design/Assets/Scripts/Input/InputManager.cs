using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputProvider inputProvider;
    private void Awake() {
        inputProvider.EnableInput();
    }

    private void OnDisable() {
        inputProvider.DisableInput();
    }
}
