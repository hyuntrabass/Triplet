using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private TileView _tilePrefab;
    [SerializeField]
    private TrayController _trayController;
    [SerializeField]
    private TileDefinition[] _tileDefinitions;
    [SerializeField]
    private BoardLevelDefinition _levelDefinition;

    private readonly List<TileView> _spawnedTiles = new();
    private int _initialTileCount;

    public bool IsEmpty => _spawnedTiles.Count == 0;
    public int RemainingTileCount => _spawnedTiles.Count;
    public float ClearProgress
    {
        get
        {
            if (_initialTileCount == 0)
            {
                return 0f;
            }

            return 1f - (float)RemainingTileCount / _initialTileCount;
        }
    }

    public event Action StateChanged;

    private void Start()
    {
        SpawnLevel();

        _initialTileCount = _spawnedTiles.Count;

        RefreshInteractableStates();
        StateChanged?.Invoke();
    }

    private void SpawnLevel()
    {
        if (_levelDefinition == null)
        {
            throw new InvalidOperationException("BoardLevelDefinition이 연결되지 않았습니다.");
        }

        if (_levelDefinition.Tiles.Count == 0)
        {
            throw new InvalidOperationException("레벨에 등록된 타일이 없습니다.");
        }

        var orderedTiles = _levelDefinition.Tiles.OrderBy(x => x.StackLevel);

        foreach (var tileData in orderedTiles)
        {
            SpawnTile(tileData.TypeId, tileData.Position, tileData.StackLevel);
        }
    }

    private void SpawnTile(int typeId, Vector2 position, int stackLevel)
    {
        TileDefinition definition = GetTileDefinition(typeId);

        var tile = Instantiate(_tilePrefab, transform);
        tile.Init(definition, stackLevel);
        tile.Clicked += HandleClicked;

        RectTransform tileRect = (RectTransform)tile.transform;
        tileRect.anchoredPosition = position;

        _spawnedTiles.Add(tile);
    }

    private TileDefinition GetTileDefinition(int typeId)
    {
        var definition = System.Array.Find(_tileDefinitions, x => x.TypeId == typeId);

        if (definition == null)
        {
            throw new System.InvalidOperationException($"TypeId {typeId}에 해당하는 TileDefinition이 없습니다.");
        }

        return definition;
    }

    public IReadOnlyList<TileView> GetExposedTiles()
    {
        return _spawnedTiles.Where(x => IsBlocked(x) == false).ToList();
    }

    public bool TryDetachTile(TileView tile)
    {
        if (tile == null || _spawnedTiles.Remove(tile) == false)
        {
            return false;
        }

        tile.Clicked -= HandleClicked;

        RefreshInteractableStates();
        StateChanged?.Invoke();

        return true;
    }

    public IReadOnlyList<int> RemoveExposedTiles()
    {
        IReadOnlyList<TileView> exposedTiles = GetExposedTiles();

        if (exposedTiles.Count == 0)
        {
            return Array.Empty<int>();
        }

        var removedTypeIds = new List<int>(exposedTiles.Count);

        foreach (var tile in exposedTiles)
        {
            removedTypeIds.Add(tile.TypeId);

            tile.Clicked -= HandleClicked;
            _spawnedTiles.Remove(tile);

            tile.gameObject.SetActive(false);
            Destroy(tile.gameObject);
        }

        RefreshInteractableStates();
        StateChanged?.Invoke();

        return removedTypeIds;
    }

    private void HandleClicked(TileView tile)
    {
        if (_trayController.TryAdd(tile) == false)
        {
            Debug.Log("수납함이 가득 찼습니다.");
            return;
        }

        TryDetachTile(tile);
    }

    private void RefreshInteractableStates()
    {
        foreach (var tile in _spawnedTiles)
        {
            tile.SetInteractable(!IsBlocked(tile));
        }
    }

    private bool IsBlocked(TileView target)
    {
        var targetRect = (RectTransform)target.transform;

        foreach (var other in _spawnedTiles)
        {
            if (other == target || other.StackLevel <= target.StackLevel)
            {
                continue;
            }

            var otherRect = (RectTransform)other.transform;

            float xDist = Mathf.Abs(targetRect.anchoredPosition.x - otherRect.anchoredPosition.x);
            float yDist = Mathf.Abs(targetRect.anchoredPosition.y - otherRect.anchoredPosition.y);

            float xOverlapLimit = (targetRect.rect.width + otherRect.rect.width) * 0.5f;
            float yOverlapLimit = (targetRect.rect.height + otherRect.rect.height) * 0.5f;

            if (xDist < xOverlapLimit &&  yDist < yOverlapLimit)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        foreach (var item in _spawnedTiles)
        {
            if (item != null)
                item.Clicked -= HandleClicked; 
        }
    }
}
