// By Night Run Studio on YT

using UnityEngine;

public class InteractablePickUp : BaseInteractable
{
    [Header("Item Data")]
    [SerializeField] private string  itemName;
    [SerializeField] private int     quantity;
    [SerializeField] private Sprite  sprite;
    [TextArea(3, 5)]
    [SerializeField] private string itemDescription;

    [Header("Message")]
    [TextArea(3, 5)]
    [SerializeField] private string  _messageText;

    private InventoryManager inventoryManager;

    protected override void Start()
    {
        base.Start();
        // cache your InventoryManager once
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
            Debug.LogWarning("InteractablePickUp: InventoryManager not found");
    }

    public override bool Interact(Interactor interactor)
    {
        // 1) Add to inventory
        inventoryManager?.AddItem(itemName, quantity, sprite, itemDescription);

        // 2) Destroy this pick‐up object
        Destroy(gameObject);

        // 3) Show your custom message
        uiManager?.ShowMessage(_messageText);

        return true;
    }
}
