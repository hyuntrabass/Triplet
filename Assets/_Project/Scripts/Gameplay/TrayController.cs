using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] _slots;

    private readonly List<TileView> _tiles = new();
    private readonly Stack<(TileView Tile, Action Restore)> _undoHistory = new();

    public bool IsEmpty => _tiles.Count == 0;
    public bool IsFull => _tiles.Count >= _slots.Length;

    public event Action StateChanged;
    public event Action<int, int> TilesRemoved;

    public bool TryAdd(TileView tile, Action restore = null)
    {
        if (IsFull)
        {
            StateChanged?.Invoke();
            return false;
        }

        int lastSameTypeIndex = _tiles.FindLastIndex(x => x.TypeId == tile.TypeId);
        int insertIndex = lastSameTypeIndex >= 0 ? lastSameTypeIndex + 1 : _tiles.Count;

        _tiles.Insert(insertIndex, tile);
        tile.SetInteractable(false);

        RearrangeTiles();
        RemoveMatchedTiles(tile.TypeId);

        if (restore == null || _tiles.Contains(tile) == false)
        {
            ClearUndoHistory();
        }
        else
        {
            _undoHistory.Push((tile, restore));
        }

        StateChanged?.Invoke();

        return true;
    }

    private void RemoveMatchedTiles(int typeId)
    {
        var matchedTiles = _tiles.FindAll(x => x.TypeId == typeId);
        if (matchedTiles.Count < 3)
        {
            return;
        }

        int removedCount = matchedTiles.Count;

        matchedTiles.ForEach(x =>
        {
            _tiles.Remove(x);
            StartCoroutine(DestroyTileNextFrame(x.gameObject));
        });

        RearrangeTiles();
        TilesRemoved?.Invoke(typeId, removedCount);
    }

    public IReadOnlyList<TileView> GetTiles()
    {
        return new List<TileView>(_tiles);
    }

    public bool TryDetachTile(TileView tile)
    {
        if (tile == null || _tiles.Remove(tile) == false)
        {
            return false;
        }

        ClearUndoHistory();
        RearrangeTiles();
        StateChanged?.Invoke();

        return true;
    }

    public void ClearAll()
    {
        ClearUndoHistory();

        _tiles.ForEach(x =>
        {
            StartCoroutine(DestroyTileNextFrame(x.gameObject));
        });

        _tiles.Clear();
        StateChanged?.Invoke();
    }

    public void ClearUndoHistory()
    {
        _undoHistory.Clear();
    }

    public bool TryUndoLastMove()
    {
        if (_undoHistory.Count == 0)
        {
            return false;
        }

        var record = _undoHistory.Peek();

        if (record.Tile == null || _tiles.Remove(record.Tile) == false)
        {
            ClearUndoHistory();
            return false;
        }

        _undoHistory.Pop();

        RearrangeTiles();
        record.Restore();

        StateChanged?.Invoke();

        return true;
    }

    private void RearrangeTiles()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].transform.SetParent(_slots[i], false);
            var rect = (RectTransform)_tiles[i].transform;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(52, 52);
        }
    }

    public List<TileView> TakeFirst(int count)
    {
        if (_tiles.Count == 0 || count <= 0)
        {
            return new List<TileView>();
        }

        int takeCount = Mathf.Min(count, _tiles.Count);

        var targets = _tiles.GetRange(0, takeCount);
        _tiles.RemoveRange(0, takeCount);
        ClearUndoHistory();

        RearrangeTiles();
        StateChanged?.Invoke();

        return targets;
    }

    private IEnumerator DestroyTileNextFrame(GameObject tileObject)
    {
        tileObject.SetActive(false);

        yield return null;

        Destroy(tileObject);
    }
}
