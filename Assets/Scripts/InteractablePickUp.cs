// By Night Run Studio on YT

using UnityEngine;

public class InteractablePickUp : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string itemName;
    [SerializeField]
    private int quantity;
    [SerializeField]
    private Sprite sprite;

    [SerializeField] private string _prompt; // Shown in interaction UI
    
    [TextArea(3, 5)]
    [SerializeField] private string _messageText; // The full sign message shown via UIManager
    public string InteractionPrompt => _prompt;
    private InventoryManager inventoryManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }
     public bool Interact(Interactor interactor)
    {
        // Destroy the game object upon interaction.
        inventoryManager.AddItem(itemName, quantity, sprite);
        Destroy(gameObject);

        // Find the UIManager in the scene
        UIManager uiManager = FindObjectOfType<UIManager>();

        if (uiManager != null)
        {
            uiManager.ShowMessage(_messageText);
        }
        else
        {
            Debug.LogWarning("UIManager not found in the scene.");
        }

        return true;
    }
}
