using System;
using UnityEngine;

[Serializable]
public class BoardTileData
{
    [SerializeField]
    private int _typeId;
    [SerializeField]
    private Vector2 _position;
    [SerializeField, Min(0)]
    private int _stackLevel;

    public int TypeId => _typeId;
    public Vector2 Position => _position;
    public int StackLevel => _stackLevel;
}
