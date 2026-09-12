using System;
using System.Collections.Generic;
using UnityEngine;

public class TempBoardController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] _columnAnchors;

    private Stack<TileView>[] _columns;
    private int _tileCount;

    public bool IsEmpty => _tileCount == 0;
    
    public event Action StateChanged;

    private void Awake()
    {
        _columns = new Stack<TileView>[_columnAnchors.Length];

        for (int i = 0; i < _columns.Length; i++)
        {
            _columns[i] = new Stack<TileView>();
        }
    }
}
