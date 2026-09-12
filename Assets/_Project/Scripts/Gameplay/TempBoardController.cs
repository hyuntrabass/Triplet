using Triplet.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TempBoardController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] _columnAnchors;
    [SerializeField]
    private TrayController _traycontroller;

    private Stack<TileView>[] _columns;
    private int _tileCount;
    private const float StackOffsetY = 5f;

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

    public void StackTiles(IReadOnlyList<TileView> tiles)
    {
        if (tiles.IsNullOrEmpty() || tiles.Count > _columns.Length)
        {
            return;
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            var column = _columns[i];
            var tile = tiles[i];

            if (column.Count > 0)
            {
                column.Peek().SetInteractable(false);
            }

            int stackLevel = column.Count;

            column.Push(tile);
            _tileCount++;

            tile.Clicked += HandleTileClicked;

            tile.transform.SetParent(_columnAnchors[i], false);

            var rect = (RectTransform)tile.transform;
            rect.anchoredPosition = Vector2.up * (StackOffsetY * stackLevel);

            tile.transform.SetAsLastSibling();
            tile.SetInteractable(true);

        }

        StateChanged?.Invoke();
    }

    private void HandleTileClicked(TileView tile)
    {
        foreach (var item in _columns)
        {
            if (item.Count == 0 || item.Peek() != tile)
            {
                continue;
            }

            if (_traycontroller.TryAdd(tile) == false)
            {
                return;
            }

            item.Pop();
            tile.Clicked -= HandleTileClicked;
            _tileCount--;

            if (item.Count > 0)
            {
                item.Peek().SetInteractable(true);
            }

            StateChanged?.Invoke();
            return;
        }
    }

    private void OnDestroy()
    {
        foreach(var column in _columns)
        {
            foreach (var tile in column)
            {
                if (tile != null)
                {
                    tile.Clicked -= HandleTileClicked;
                }
            }
        }
    }
}
