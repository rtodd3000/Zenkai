// By Night Run Studio on YT

using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject followCameraGO;

    private bool menuActivated = false;
    public ItemSlot[] itemSlot;

    [Header("Currency (Wisps)")]
    public int WispCurrency { get; private set; } = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (menuActivated) CloseInventory();
            else             OpenInventory();
        }
    }

    // Fully open inventory (pause the game)
    private void OpenInventory()
    {
        menuActivated = true;
        Time.timeScale = 0f;
        InventoryMenu.SetActive(true);
        if (followCameraGO != null) followCameraGO.SetActive(false);

        // SHOW the mouse cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // If the game is already paused, just hide the pause UI
        var pause = FindObjectOfType<PauseMenu>();
        if (pause != null && PauseMenu.paused)
            pause.HideUIOnly();
    }

    // Fully close inventory (resume only if pause isn't active)
    private void CloseInventory()
    {
        menuActivated = false;
        InventoryMenu.SetActive(false);

        if (!PauseMenu.paused)
        {
            // No pause-menu open, so actually unpause
            Time.timeScale = 1f;
            if (followCameraGO != null) followCameraGO.SetActive(true);

            // HIDE the mouse cursor again
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Pause-menu is still active, so re-show it
            var pause = FindObjectOfType<PauseMenu>();
            if (pause != null)
                pause.ShowUIOnly();
        }
    }

    // Just hide the inventory UI without touching timeScale or camera
    public void HideUIOnly()
    {
        if (!menuActivated) return;
        menuActivated = false;
        InventoryMenu.SetActive(false);
    }

    /// <summary>Add wisps to your currency total.</summary>
    public void AddCurrency(int amount)
    {
        WispCurrency += amount;
        Debug.Log($"[Currency] Added {amount}, total now {WispCurrency}");
        // (Optionally update a UI text somewhere)
    }

    /// <summary>Try to spend wisps; returns true on success.</summary>
    public bool SpendCurrency(int amount)
    {
        Debug.Log($"[Currency] Attempting to spend {amount}, you have {WispCurrency}");
        if (WispCurrency >= amount)
        {
            WispCurrency -= amount;
            Debug.Log($"[Currency] Spent {amount}, remaining {WispCurrency}");
            return true;
        }
        return false;
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemName)
            {
                // Found same item—just bump quantity
                itemSlot[i].IncreaseQuantity(quantity);
                return;
            }
        }

        for(int i = 0; i < itemSlot.Length; i++)
        {
            if(itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {

            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }
}
