using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Shop/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Core Info")]
    public string upgradeName;
    [TextArea(3,5)]
    public string upgradeDescription;
    public Sprite icon;

    [Header("Gameplay")]
    public int basePrice;
    public int maxStack = 1;
}
