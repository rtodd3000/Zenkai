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
        // 1) Always add the item to the grid
        inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);

        if (isCurrencyPickup)
        {
            // 2) Immediately re-sync your wisp balance from the inventory slots
            inventoryManager.SyncCurrencyFromInventory(itemName);

            // 3) Show exactly how many wisps you now have
            uiManager?.ShowMessage(
                $"You picked up {quantity} wisps! " +
                $" Total Wisps: {inventoryManager.WispCurrency}"
            );
        }
        else
        {
            uiManager?.ShowMessage(_messageText);
        }

        // 4) Finally destroy the world object
        Destroy(gameObject);
        return true;
    }
}
