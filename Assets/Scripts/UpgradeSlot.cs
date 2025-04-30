using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using StarterAssets;  // namespace for the ThirdPersonController

public class UpgradeSlot : MonoBehaviour, IPointerClickHandler
{
    public enum UpgradeType { Speed, Jump }

    [Header("Data")]
    public string      upgradeName;
    public int         quantity;
    public Sprite      upgradeSprite;
    public int         cost   = 5;
    public UpgradeType type   = UpgradeType.Speed;
    public float       amount = 0.5f;  // how much to bump per purchase

    [Header("Slot UI")]
    [SerializeField] private Image    upgradeImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private GameObject selectedShader;
    public bool thisItemSelected;

    [Header("Description Prefab")]
    [SerializeField] private GameObject myDescriptionPanel;

    // Cached references
    private ShopManager            shopManager;
    private InventoryManager       inventoryManager;
    private UIManager              uiManager;
    private ThirdPersonController  playerController;

    private void Start()
    {
        shopManager       = FindObjectOfType<ShopManager>();
        inventoryManager  = FindObjectOfType<InventoryManager>();
        uiManager         = FindObjectOfType<UIManager>();
        playerController  = FindObjectOfType<ThirdPersonController>();

        upgradeImage.sprite = upgradeSprite;
        Deselect();
        myDescriptionPanel.SetActive(false);
        UpdateQuantityUI();
    }

    public void IncreaseQuantity(int delta)
    {
        quantity += delta;
        UpdateQuantityUI();
    }

    private void UpdateQuantityUI()
    {
        quantityText.text = quantity.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
    }

    public void OnLeftClick()
    {
        shopManager?.DeselectAllDescriptions();
        shopManager?.DeselectAllSlots();

        selectedShader.SetActive(true);
        thisItemSelected = true;

        myDescriptionPanel.SetActive(true);
    }

    private void OnRightClick()
    {
        // Try to spend the wisps
        if (inventoryManager != null && inventoryManager.SpendCurrency(cost))
        {
            // Apply the actual upgrade effect
            if (playerController != null)
            {
                switch (type)
                {
                    case UpgradeType.Speed:
                        // These are the public fields in StarterAssets' ThirdPersonController
                        playerController.MoveSpeed   += amount;
                        playerController.SprintSpeed += amount;
                        break;
                    case UpgradeType.Jump:
                        playerController.JumpHeight += amount;
                        break;
                }
            }

            // Track how many you've bought
            IncreaseQuantity(1);

            uiManager?.ShowMessage(
                $"Bought {upgradeName} for {cost} wisps! You now have {quantity}. Remaining Wisps: {inventoryManager.WispCurrency}"
            );
        }
        else
        {
            uiManager?.ShowMessage("Not enough wisps!");
        }
    }

    public void Deselect()
    {
        thisItemSelected = false;
        selectedShader.SetActive(false);
    }
}
