using System;
using System.Collections.Generic;
using Triplet.Core;
using UnityEngine;

public class TempBoardController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] _columnAnchors;
    [SerializeField]
    private TrayController _trayController;

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

    public IReadOnlyList<TileView> GetTopTiles()
    {
        var topTiles = new List<TileView>();

        foreach (var column in _columns)
        {
            if (column.Count > 0)
            {
                topTiles.Add(column.Peek());
            }
        }

        return topTiles;
    }

    public IReadOnlyList<TileView> GetAllTiles()
    {
        var tiles = new List<TileView>();

        foreach (var column in _columns)
        {
            tiles.AddRange(column);
        }

        return tiles;
    }

    public bool TryDetachTile(TileView tile)
    {
        if (tile == null)
        {
            return false;
        }

        foreach (var column in _columns)
        {
            if (column.Count == 0 || column.Peek() != tile)
            {
                continue;
            }

            column.Pop();
            tile.Clicked -= HandleTileClicked;
            _tileCount--;

            if (column.Count > 0)
            {
                column.Peek().SetInteractable(true);
            }

            StateChanged?.Invoke();
            return true;
        }

        return false;
    }

    private void RestoreTile(TileView tile, int columnIndex, Vector2 position, Vector2 sizeDelta)
    {
        var column = _columns[columnIndex];

        if (column.Count > 0)
        {
            column.Peek().SetInteractable(false);
        }

        column.Push(tile);
        _tileCount++;

        var rect = (RectTransform)tile.transform;

        rect.SetParent(_columnAnchors[columnIndex], false);
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;
        rect.SetAsLastSibling();

        tile.Clicked += HandleTileClicked;
        tile.SetInteractable(true);
        StateChanged?.Invoke();
    }

    private void HandleTileClicked(TileView tile)
    {
        int columnIndex = Array.FindIndex(_columns, x => x.Count > 0 && x.Peek() == tile);

        if (columnIndex < 0)
        {
            return;
        }

        var rect = (RectTransform)tile.transform;

        Vector2 position = rect.anchoredPosition;
        Vector2 sizeDelta = rect.sizeDelta;

        var added = _trayController.TryAdd(tile, () => RestoreTile(tile, columnIndex, position, sizeDelta));

        if (added == false)
        {
            return;
        }

        TryDetachTile(tile);
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
