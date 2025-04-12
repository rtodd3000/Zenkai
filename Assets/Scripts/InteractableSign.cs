using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSign : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Reading Sign!");
        
        // Find the UIManager in the scene and display the sign's message.
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowMessage("Welcome to Zenkai! Explore this area and find all 7 Wisps!");
        }
        else
        {
            Debug.LogWarning("UIManager not found in the scene.");
        }
        
        return true;
    }
}
