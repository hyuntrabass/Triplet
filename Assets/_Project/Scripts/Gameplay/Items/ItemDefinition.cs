using UnityEngine;

[CreateAssetMenu(
    fileName ="ItemDefinition",
    menuName ="Triplet/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField]
    private ItemType _type;
    [SerializeField]
    private string _displayName;
    [SerializeField]
    private Sprite _icon;

    public ItemType Type => _type;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
}
