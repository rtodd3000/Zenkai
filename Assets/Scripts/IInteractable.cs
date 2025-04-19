// by Dan Pos on YT

using UnityEngine;

public interface IInteractable
{
    public string InteractionPrompt { get; }
    public bool Interact(Interactor interactor);
}

