using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Change this for different interactable object
public class InteractableSign : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;

    // Change this for different interactable object
    public bool Interact(Interactor interactor)
    {
        Debug.Log(message: "Reading Sign!");
        return true;
    }
}
