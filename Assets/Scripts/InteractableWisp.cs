using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableWisp : BaseInteractable
{
    [Header("Wisp Message")]
    [TextArea(2,4)]
    [SerializeField] private string _collectMessage = "You got a Wisp!";

    private static int wispCount = 0;

    // Called before Start; we use this to gate the whole component
    private void Awake()
    {
        // If we're not in scene index 2, turn ourselves off entirely
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            enabled = false;
        }
    }

    // BaseInteractable.Start() will cache UIManager for us already

    public override bool Interact(Interactor interactor)
    {
        // Only runs if Awake left us enabled (i.e. scene == 2)

        wispCount++;
        if (wispCount == 7)
        {
            uiManager?.ShowMessage("You collected 7 wisps! Demo is Complete");
        }
        else
        {
            uiManager?.ShowMessage(_collectMessage);
        }

        return true;
    }
}
