// By Night Run Studio on YT

using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("UI & Camera")]
    public GameObject ShopMenu;
    public GameObject followCameraGO;

    [Header("Slots")]
    public UpgradeSlot[] upgradeSlots;

    [Header("Description Panels")]
    public GameObject[] descriptionPanels;

    private bool menuActivated = false;

    private void Update()
    {
        if (!menuActivated) return;

        // Close shop if player presses E, Escape, or Tab
        if (Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Tab))
        {
            CloseShop();
        }
    }

    /// <summary>Hide every description prefab.</summary>
    public void DeselectAllDescriptions()
    {
        foreach (var panel in descriptionPanels)
            panel.SetActive(false);
    }
    
    /// <summary>Show shop UI (without pausing game).</summary>
    public void OpenShop()
    {
        if (menuActivated) return;
        menuActivated = true;

        ShopMenu.SetActive(true);
        followCameraGO?.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>Hide shop UI and restore camera/cursor.</summary>
    public void CloseShop()
    {
        if (!menuActivated) return;
        menuActivated = false;

        ShopMenu.SetActive(false);
        followCameraGO?.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>Hide shop UI if you need to from elsewhere.</summary>
    public void HideUIOnly()
    {
        if (!menuActivated) return;
        menuActivated = false;
        ShopMenu.SetActive(false);
    }

    /// <summary>Deselect all slot highlights.</summary>
    public void DeselectAllSlots()
    {
        foreach (var slot in upgradeSlots)
            slot.Deselect();
    }
}
