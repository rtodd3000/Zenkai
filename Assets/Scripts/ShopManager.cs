// By Night Run Studio on YT

// By Night Run Studio on YT

using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("UI & Camera")]
    public GameObject ShopMenu;       // Your shop UI panel
    public GameObject followCameraGO; // Cinemachine vcam or similar

    [Header("Slots")]
    public UpgradeSlot[] upgradeSlots;

    private bool menuActivated = false;

    void Update()
    {
        // Still allow Tab to toggle shop if you want
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (menuActivated) CloseShop();
            else             OpenShop();
        }
    }

    /// <summary>
    /// Opens the shop: pauses game, shows UI, hides camera, shows cursor.
    /// </summary>
    public void OpenShop()
    {
        if (menuActivated) return;

        menuActivated = true;
        Time.timeScale = 0f;
        ShopMenu.SetActive(true);
        if (followCameraGO != null) followCameraGO.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // If the game is paused by the pause menu, hide that UI
        var pause = FindObjectOfType<PauseMenu>();
        if (pause != null && PauseMenu.paused)
            pause.HideUIOnly();
    }

    /// <summary>
    /// Closes the shop: optionally unpauses, hides UI, re-enables camera, hides cursor.
    /// </summary>
    public void CloseShop()
    {
        if (!menuActivated) return;

        menuActivated = false;
        ShopMenu.SetActive(false);

        // Only unpause if the pause menu isn't active
        if (!PauseMenu.paused)
        {
            Time.timeScale = 1f;
            if (followCameraGO != null) followCameraGO.SetActive(true);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // If pause is still active, re‐show its UI
            var pause = FindObjectOfType<PauseMenu>();
            pause?.ShowUIOnly();
        }
    }

    /// <summary>
    /// Hides just the shop UI (used by PauseMenu).
    /// </summary>
    public void HideUIOnly()
    {
        if (!menuActivated) return;
        menuActivated = false;
        ShopMenu.SetActive(false);
    }

    /// <summary>
    /// Helper to add items into the shop’s item slots (if you want to populate items at runtime).
    /// </summary>
    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // identical stacking logic to your inventory...
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (upgradeSlots[i].isFull && upgradeSlots[i].itemName == itemName)
            {
                upgradeSlots[i].IncreaseQuantity(quantity);
                return;
            }
        }
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (!upgradeSlots[i].isFull)
            {
                upgradeSlots[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
        }
    }
}
