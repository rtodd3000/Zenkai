using System.Collections;
using UnityEngine;

public class InteractableSign : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt; // Shown in interaction UI
    
    [TextArea(3, 5)]
    [SerializeField] private string _messageText; // The full sign message shown via UIManager

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        // debug tool for testing
        Debug.Log("Reading Sign!");

        // Find the UIManager in the scene and display the corresponding message.
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            // a prompt in the gameobject to easily test
            uiManager.ShowMessage(_messageText);
        }
        else
        {
            // debug tool for testing
            Debug.LogWarning("UIManager not found in the scene.");
        }

        return true;
    }
}
