// By Night Run Studio on YT

using UnityEngine;

public class InteractablePickUp : BaseInteractable
{
    [Header("Item Data")]
    [SerializeField] private string itemName;
    [SerializeField] private int quantity;
    [SerializeField] private Sprite sprite;
    [TextArea(3, 5)]
    [SerializeField] private string itemDescription;

    [Header("Quest Flags")]
    [SerializeField] private bool isBook = false;

    [Header("Behavior")]
    [Tooltip("If true, this pickup adds wisps (currency) instead of a normal item")]
    [SerializeField] private bool isCurrencyPickup = false;

    [SerializeField] private GameObject crownPrefab;       // Crown asset to attach
    [SerializeField] private Transform headTransform;      // Character's head transform

    [Header("Message")]
    [TextArea(3, 5)]
    [SerializeField] private string _messageText;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip pickupSound; // <--- Add this in Inspector

    private InventoryManager inventoryManager;

    protected override void Start()
    {
        base.Start();
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
            Debug.LogWarning("InteractablePickUp: InventoryManager not found");
    }

    public override bool Interact(Interactor interactor)
    {
        Debug.Log("Interact() called on " + gameObject.name);

        inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);

        if (isCurrencyPickup)
        {
            inventoryManager.SyncCurrencyFromInventory(itemName);
            uiManager?.ShowMessage($"You picked up {quantity} wisps! Total Wisps: {inventoryManager.WispCurrency}");
        }
        else
        {
            uiManager?.ShowMessage(_messageText);
        }

        if (isBook && BookTracker.Instance != null)
        {
            BookTracker.Instance.BookCollected();
        }

        // 🔊 Play pickup sound at the item's position
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        Destroy(gameObject);
        return true;
    }
}
