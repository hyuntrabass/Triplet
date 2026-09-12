using System;
using System.Collections.Generic;
using UnityEngine;

public class TempTrayController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] _slots;

    private readonly List<TileView> _tiles = new();

    public bool IsEmpty => _tiles.Count == 0;
    
    public event Action StateChanged;
}
