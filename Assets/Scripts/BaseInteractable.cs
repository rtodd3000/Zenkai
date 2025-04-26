using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    protected UIManager uiManager;
    public string InteractionPrompt => _prompt;

    protected virtual void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
            Debug.LogWarning("BaseInteractable: UIManager not found in scene");
    }

    public abstract bool Interact(Interactor interactor);
}
