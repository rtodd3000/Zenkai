using UnityEngine;

public class InteractableObelisk : BaseInteractable
{
    [TextArea(3,5)]
    [SerializeField] private string _messageText; // e.g. “Welcome to the shop!”

    private ShopManager shopManager;

    protected override void Start()
    {
        base.Start();
        shopManager = FindObjectOfType<ShopManager>();
        if (shopManager == null)
            Debug.LogWarning("InteractableObelisk: ShopManager not found");
    }

    public override bool Interact(Interactor interactor)
    {
        uiManager?.ShowMessage(_messageText);
        shopManager?.OpenShop();
        return true;
    }
}