using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeDescriptionPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;      // The root panel GameObject
    [SerializeField] private TMP_Text nameText;         // Drag NameText here
    [SerializeField] private TMP_Text descriptionText;  // Drag DescText here
    [SerializeField] private Image iconImage;           // Drag IconImage here

    private void Awake()
    {
        // Ensure it's hidden on start
        panelRoot.SetActive(false);
    }

    /// <summary>
    /// Shows the panel and fills in the data.
    /// </summary>
    public void Show(string itemName, string itemDesc, Sprite icon)
    {
        nameText.text        = itemName;
        descriptionText.text = itemDesc;
        iconImage.sprite     = icon;
        panelRoot.SetActive(true);
    }

    /// <summary>
    /// Hides the panel.
    /// </summary>
    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}
