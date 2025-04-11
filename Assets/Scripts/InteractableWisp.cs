using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Change this for different interactable object
public class InteractableWisp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;

    // Change this for different interactable object
    public bool Interact(Interactor interactor)
    {
        Destroy(gameObject);
        Debug.Log(message: "Get Wisp!");
        return true;
    }
}
