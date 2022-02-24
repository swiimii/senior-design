using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public interface IInteractable {
    float HoldDuration { get; }
    bool HoldInteract { get; }
    bool MultipleUse { get; }
    bool IsInteractable { get; }

    void OnInteract();
}

public class InteractableBase : MonoBehaviour, IInteractable {
    [Header("Interactable Settings"), Tooltip("Make very big if not work"), SerializeField]
    private float requiredDistance;

    [Space, SerializeField] private bool isSpecialInteraction;
    [Space, SerializeField] private float holdDuration;
    [SerializeField] private bool holdInteract;
    [SerializeField] private bool multipleUse;
    [SerializeField] private bool isInteractable;

    public float RequiredDistance => requiredDistance;
    public float HoldDuration => holdDuration;
    public bool HoldInteract => holdInteract;
    public bool MultipleUse => multipleUse;
    public bool IsInteractable => isInteractable;
    public bool IsSpecialInteraction => isSpecialInteraction;

    public virtual void OnInteract() {
        Helper.CustomLog("INTERACTED: " + gameObject.name, LogColor.White);
    }
}