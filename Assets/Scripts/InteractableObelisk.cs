using UnityEngine;

public class InteractableObelisk : BaseInteractable
{
    [TextArea(3,5)]
    [SerializeField] private string _messageText; // The message to show

    public override bool Interact(Interactor interactor)
    {
        Debug.Log("Reading Obelisk!");
        uiManager?.ShowMessage(_messageText);
        return true;
    }
}
