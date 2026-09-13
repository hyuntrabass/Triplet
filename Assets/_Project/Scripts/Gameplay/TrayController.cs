using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] _slots;

    private readonly List<TileView> _tiles = new();

    public bool IsEmpty => _tiles.Count == 0;
    public bool IsFull => _tiles.Count >= _slots.Length;

    public event Action StateChanged;
    public event Action<int, int> TilesRemoved;

    public bool TryAdd(TileView tile)
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

        RearrangeTiles();
        StateChanged?.Invoke();

        return true;
    }

    public void ClearAll()
    {
        _tiles.ForEach(x =>
        {
            StartCoroutine(DestroyTileNextFrame(x.gameObject));
        });

        _tiles.Clear();
        StateChanged?.Invoke();
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
