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
    [SerializeField]
    [TextArea(2, 4)]
    private string _description;

    public ItemType Type => _type;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public string Description => _description;
}
