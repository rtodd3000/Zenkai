using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableWisp : BaseInteractable
{
    [Header("Wisp Messages")]
    [TextArea(2,4)]
    [SerializeField] private string _collectMessage  = "You got a Wisp!";  
    [TextArea(2,4)]
    [SerializeField] private string _completeMessage = "You collected 7 wisps! Demo is Complete!";

    private InventoryManager _inventoryManager;

    private void Awake()
    {
        // Only enable in scene index 2 (tutorial/demo)
        if (SceneManager.GetActiveScene().buildIndex != 2)
            enabled = false;
    }

    protected override void Start()
    {
        base.Start();  // caches uiManager for you

        // Cache InventoryManager so we can add currency
        _inventoryManager = FindObjectOfType<InventoryManager>();
        if (_inventoryManager == null)
            Debug.LogWarning("InteractableWisp: InventoryManager not found in scene");
    }

    public override bool Interact(Interactor interactor)
    {
        // Destroy the wisp in the world
        Destroy(gameObject);

        // Add 1 wisp to the global currency
        _inventoryManager?.AddCurrency(1);

        // Fetch the new total
        int total = _inventoryManager != null
            ? _inventoryManager.WispCurrency
            : 0;

        // If this was the 7th, show complete message
        if (total == 7)
        {
            uiManager?.ShowMessage(_completeMessage);
        }
        else
        {
            // Otherwise show the collect prompt + current total
            uiManager?.ShowMessage($"{_collectMessage}\nWisps: {total}");
        }

        return true;
    }
}
