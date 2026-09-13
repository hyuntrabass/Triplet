using UnityEngine;

[CreateAssetMenu(fileName = "TileDefinition", menuName = "Triplet/Tile Definition")]
public sealed class TileDefinition : ScriptableObject
{
    [SerializeField, Min(1)]
    private int _typeId;
    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private Color _placeholderColor = Color.white;

    public int TypeId => _typeId;
    public Sprite Sprite => _sprite;
    public Color PlaceholderColor => _placeholderColor;
}
