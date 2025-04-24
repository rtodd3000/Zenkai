using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableWisp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    // This message will be shown when collecting wisps in normal gameplay scenes
    [TextArea(2, 4)]
    [SerializeField] private string _collectMessage = "You got a Wisp!";

    // Static variable to track the total number of collected wisps.
    private static int wispCount = 0;

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        // Destroy the wisp game object upon interaction.
        Destroy(gameObject);
        Debug.Log("Get Wisp!");

        // Find the UIManager in the scene
        UIManager uiManager = FindObjectOfType<UIManager>();

        // Checks if scene is in the Main Game Index (non-tutorial gameplay)
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (uiManager != null)
            {
                uiManager.ShowMessage(_collectMessage);
            }
            else
            {
                Debug.LogWarning("UIManager not found in the scene.");
            }

            Debug.Log("In Non-Tutorial Index");
            // will add more stuff here relating to the inventory
        }

        // Checks if the scene is the Tutorial / Demo Index
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            // Increase the global count of collected wisps.
            wispCount++;

            if (uiManager != null)
            {
                if (wispCount == 7)
                {
                    uiManager.ShowMessage("You collected 7 wisps! Demo is Complete, Feel Free to Explore");
                }
                else
                {
                    uiManager.ShowMessage("You got a wisp! Total wisps: " + wispCount);
                }
            }
            else
            {
                Debug.LogWarning("UIManager not found in the scene.");
            }
        }

        return true;
    }
}
