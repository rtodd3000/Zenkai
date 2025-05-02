using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableWisp : BaseInteractable
{
    [Header("Wisp Messages")]
    [TextArea(2,4)]
    [SerializeField] private string _collectMessage  = "You got a Wisp!";  
    [TextArea(2,4)]
    [SerializeField] private string _completeMessage = "You collected 43 wisps! You deserve a crown for that! Thank you for supporting Zenkai";

    [Header("Crown Attachment")]
    [SerializeField] private GameObject crownPrefab;
    [SerializeField] private Transform headTransform;

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

        _inventoryManager = FindObjectOfType<InventoryManager>();
        if (_inventoryManager == null)
            Debug.LogWarning("InteractableWisp: InventoryManager not found in scene");
    }

    public override bool Interact(Interactor interactor)
    {
        Debug.Log("[Wisp] Interact() called on " + gameObject.name);

        // Destroy the wisp in the world
        Destroy(gameObject);

        // Add 1 wisp to the global currency
        _inventoryManager?.AddCurrency(1);

        // Fetch the new total
        int total = _inventoryManager != null ? _inventoryManager.WispCurrency : 0;

        // If this was the 43rd, show complete message and give crown
        if (total == 43)
        {
            uiManager?.ShowMessage(_completeMessage);

            if (crownPrefab != null && headTransform != null)
            {
                // Prevent multiple crowns
                bool alreadyHasCrown = headTransform.GetComponentInChildren<CrownTag>() != null;
                if (!alreadyHasCrown)
                {
                    GameObject crownInstance = Instantiate(crownPrefab, headTransform);
                    crownInstance.transform.localPosition = new Vector3(0.05f, 0.15f, 0f);
                    crownInstance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    crownInstance.transform.localScale = Vector3.one * 0.75f;

                    // Add a marker script or tag to identify the crown
                    crownInstance.AddComponent<CrownTag>();
                }
            }
        }
        else
        {
            // Otherwise show the collect message + total
            uiManager?.ShowMessage($"{_collectMessage}\nWisps: {total}");
        }

        return true;
    }

    // Optional marker class to help prevent duplicate crowns
    private class CrownTag : MonoBehaviour {}
}
