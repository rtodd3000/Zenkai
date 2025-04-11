using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableWisp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    // Static variable to track the total number of collected wisps.
    private static int wispCount = 0;

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        // Destroy the wisp game object upon interaction.
        Destroy(gameObject);
        Debug.Log("Get Wisp!");

        // Increase the global count of collected wisps.
        wispCount++;

        // Find the UIManager in the scene and display the corresponding message.
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            if (wispCount == 7)
            {
                // If exactly 7 wisps have been collected, show a special message.
                uiManager.ShowMessage("You collected 7 wisps! Demo is Complete, Feel Free to Explore");
            }
            else
            {
                // Otherwise, show the normal message with the current count.
                uiManager.ShowMessage("You got a wisp! Total wisps: " + wispCount);
            }
        }
        else
        {
            Debug.LogWarning("UIManager not found in the scene.");
        }

        return true;
    }
}
