using UnityEngine;

public class InteractableSign : BaseInteractable
{
    [TextArea(3,5)]
    [SerializeField] private string _messageText; // The message to show

    public override bool Interact(Interactor interactor)
    {
        Debug.Log("Reading Sign!");
        uiManager?.ShowMessage(_messageText);
        return true;
    }
}
