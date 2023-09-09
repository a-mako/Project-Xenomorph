using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject {
    public enum ItemType {
        Weapon,
        Consumable,
        KeyItem
    }
    public string Name => name;
    public List<string> Description => description;
    public Image Sprite => sprite;
    public ItemType Type => type;

    [SerializeField] new private string name;
    [SerializeField] private List<string> description;
    [SerializeField] private Image sprite;
    [SerializeField] private ItemType type;
}
