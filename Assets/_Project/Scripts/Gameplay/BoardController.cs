using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private TileView _tilePrefab;
    [SerializeField]
    private TrayController _trayController;
    private bool _isInputEnabled = true;

    private readonly List<TileView> _spawnedTiles = new();

    public bool IsEmpty => _spawnedTiles.Count == 0;

    public event Action StateChanged;

    private void Start()
    {
        SpawnTile(1, new Vector2(0, 30), 0);
        SpawnTile(1, new Vector2(70, 30), 0);

        SpawnTile(4, new Vector2(-70, -40), 0);
        SpawnTile(5, new Vector2(0, -40), 0);
        SpawnTile(6, new Vector2(70, -40), 0);

        SpawnTile(1, new Vector2(-70, 0), 1);
        SpawnTile(2, new Vector2(0, 0), 1);
        SpawnTile(3, new Vector2(70, 0), 1);

        RefreshInteractableStates();
    }

    private void SpawnTile(int typeId, Vector2 position, int stackLevel)
    {
        var tile = Instantiate(_tilePrefab, transform);
        tile.Init(typeId, stackLevel);
        tile.Clicked += HandleClicked;

        RectTransform tileRect = (RectTransform)tile.transform;
        tileRect.anchoredPosition = position;

        _spawnedTiles.Add(tile);
    }

    private void HandleClicked(TileView tile)
    {
        if (_trayController.TryAdd(tile) == false)
        {
            Debug.Log("수납함이 가득 찼습니다.");
            return;
        }

        tile.Clicked -= HandleClicked;
        _spawnedTiles.Remove(tile);
        RefreshInteractableStates();

        StateChanged?.Invoke();
    }

    private void RefreshInteractableStates()
    {
        foreach (var tile in _spawnedTiles)
        {
            tile.SetInteractable(_isInputEnabled && !IsBlocked(tile));
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

    public void SetInputEnabled(bool value)
    {
        _isInputEnabled = value;
        RefreshInteractableStates();
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
