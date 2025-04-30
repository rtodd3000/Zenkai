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
        // 1) If this is a wisp/currency, add to your wisp balance:
        if (isCurrencyPickup)
        {
            inventoryManager.AddCurrency(quantity);
            uiManager?.ShowMessage(
                $"You picked up {quantity} wisps! Total Wisps: {inventoryManager.WispCurrency}"
            );
            inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            uiManager?.ShowMessage(_messageText);
        }
        else
        {
            // 2) Otherwise treat it like a normal item
            inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            uiManager?.ShowMessage(_messageText);
        }

        // 3) Destroy the world object and return
        Destroy(gameObject);
        return true;
    }
}
