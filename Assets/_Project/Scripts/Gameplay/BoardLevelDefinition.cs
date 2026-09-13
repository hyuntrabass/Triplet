using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardLevel", menuName = "Triplet/Board Level")]
public class BoardLevelDefinition : ScriptableObject
{
    [SerializeField]
    private BoardTileData[] _tiles = Array.Empty<BoardTileData>();

    public IReadOnlyList<BoardTileData> Tiles => _tiles;
}
