using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IPointerClickHandler
{
    // Upgrade Data
    public string upgradeName;
    public int quantity;
    public Sprite upgradeSprite;
    public bool isFull;
    public string upgradeDescription;

    // UI References
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image    upgradeImage;
    [SerializeField] private GameObject selectedShader;

    // Centralized description panel
    private UpgradeDescriptionPanel descPanel;

    private void Start()
    {
        // Cache the description panel in the scene
        descPanel = FindObjectOfType<UpgradeDescriptionPanel>();
        if (descPanel == null)
            Debug.LogWarning("UpgradeSlot: No UpgradeDescriptionPanel found in scene");
    }

    /// <summary>
    /// Initializes this slot with upgrade data.
    /// </summary>
    public void AddItem(string upgradeName, int quantity, Sprite upgradeSprite, string upgradeDescription)
    {
        this.upgradeName        = upgradeName;
        this.quantity           = quantity;
        this.upgradeSprite      = upgradeSprite;
        this.upgradeDescription = upgradeDescription;
        isFull                  = true;

        upgradeImage.sprite     = upgradeSprite;
        quantityText.text       = this.quantity.ToString();
        quantityText.enabled    = true;
    }

    /// <summary>
    /// Increases the quantity of this upgrade in the slot.
    /// </summary>
    public void IncreaseQuantity(int amount)
    {
        quantity += amount;
        quantityText.text = quantity.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
    }

    /// <summary>
    /// Handles left‐click: highlights slot and shows description.
    /// </summary>
    private void OnLeftClick()
    {
        // Deselect other slots
        FindObjectOfType<ShopManager>()?.DeselectAllSlots();
        selectedShader.SetActive(true);

        // Display this upgrade’s details in the central panel
        descPanel?.Show(upgradeName, upgradeDescription, upgradeSprite);
    }
}
