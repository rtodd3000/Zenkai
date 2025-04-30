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

    [Header("Behavior")]
    [Tooltip("If true, this pickup adds wisps (currency) instead of a normal item")]
    [SerializeField] private bool    isCurrencyPickup = false;

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
        Debug.Log("Interact() called on " + gameObject.name);

        // 1) Add to your inventory UI (so wisp shows up as an item, if you want)
        inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);

        // 2) If it’s a wisp, also treat it as currency
        if (isCurrencyPickup)
        {
            inventoryManager.AddCurrency(quantity);
            uiManager?.ShowMessage(
                $"You picked up {quantity} wisps! " +
                $" Total Wisps: {inventoryManager.WispCurrency}"
            );
        }
        else
        {
            // Normal item pickup message
            uiManager?.ShowMessage(_messageText);
        }

        // 3) Destroy the pickup in the world
        Destroy(gameObject);
        return true;
    }
}
