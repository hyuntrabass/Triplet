using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private BoardController _boardController;
    [SerializeField]
    private TrayController _trayController;
    [SerializeField]
    private TempBoardController _tempBoardController;
    [SerializeField]
    private ItemDefinition[] _itemLoadout;
    [SerializeField]
    private string _displayName;

    private ItemInventory _itemInventory;

    public bool IsDown => _trayController.IsFull;
    public bool IsClear => _boardController.IsEmpty && _tempBoardController.IsEmpty;
    public IReadOnlyList<ItemState> ItemStates => _itemInventory.States;
    public float ClearProgress => _boardController.ClearProgress;
    public string DisplayName => _displayName;
    public PlayerStatistics Statistics { get; } = new();

    public event Action StateChanged;
    public event Action<PlayerController, int, int> OrderTilesRemoved;
    public event Action<int, int> TilesMatched;

    private void Awake()
    {
        _itemInventory = new ItemInventory(_itemLoadout);
    }

    private void OnEnable()
    {
        _boardController.StateChanged += HandleStateChanged;
        _trayController.StateChanged += HandleStateChanged;
        _tempBoardController.StateChanged += HandleStateChanged;
        _trayController.TilesRemoved += HandleTilesRemoved;
    }

    public ItemState GrantRandomItem()
    {
        return _itemInventory.GrantRandomItem();
    }

    private void HandleStateChanged()
    {
        StateChanged?.Invoke();
    }

    private void HandleTilesRemoved(int typeId, int count)
    {
        OrderTilesRemoved?.Invoke(this, typeId, count);
        TilesMatched?.Invoke(typeId, count);
    }

    public IReadOnlyList<TileView> GetWarehouseCandidates()
    {
        var candidates = new List<TileView>();

        candidates.AddRange(_boardController.GetExposedTiles());
        candidates.AddRange(_trayController.GetTiles());
        candidates.AddRange(_tempBoardController.GetTopTiles());

        return candidates;
    }

    private bool TryUseHammer()
    {
        IReadOnlyList<int> removedTypeIds = _boardController.RemoveExposedTiles();

        if (removedTypeIds.Count == 0)
        {
            return false;
        }

        foreach (var group in removedTypeIds.GroupBy(x => x))
        {
            OrderTilesRemoved?.Invoke(this, group.Key, group.Count());
        }

        return true;
    }

    private bool TryUseShuffle()
    {
        var tiles = new List<TileView>();

        tiles.AddRange(_boardController.GetAllTiles());
        tiles.AddRange(_tempBoardController.GetAllTiles());

        var originalTypeIds = tiles.Select(x => x.TypeId).ToList();

        if (originalTypeIds.Distinct().Count() < 2)
        {
            return false;
        }

        var definitions = tiles.Select(x => x.Definition).ToList();

        do
        {
            for (int i = definitions.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                var temp = definitions[i];
                definitions[i] = definitions[randomIndex];
                definitions[randomIndex] = temp;
            }
        }
        while (definitions.Select(x => x.TypeId).SequenceEqual(originalTypeIds));

        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            tile.Init(definitions[i], tile.StackLevel);
        }

        StateChanged?.Invoke();

        return true;
    }

    public void CancelFreeSelect()
    {
        _boardController.CancelFreeSelect();
    }

    public bool TryDetachTile(TileView tile)
    {
        if (_boardController.TryDetachTile(tile))
        {
            return true;
        }

        if (_trayController.TryDetachTile(tile))
        {
            return true;
        }

        return _tempBoardController.TryDetachTile(tile);
    }

    public void ClearMainTray()
    {
        _trayController.ClearAll();
    }

    public void ClearUndoHistory()
    {
        _trayController.ClearUndoHistory();
    }    

    public bool TryAddToTray(TileView tile)
    {
        if (tile == null)
        {
            throw new ArgumentNullException(nameof(tile));
        }

        return _trayController.TryAdd(tile);
    }

    public bool TryUseItem(ItemState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (_itemInventory.Contains(state) == false)
        {
            Debug.LogWarning("다른 플레이어의 아이템은 사용할 수 없습니다.");
            return false;
        }

        if (state.CanUse == false)
        {
            return false;
        }

        ItemType itemType = state.Definition.Type;

        if (itemType == ItemType.FreeSelect)
        {
            return _boardController.TryBeginFreeSelect(() => 
            {
                ClearUndoHistory();
                state.TryConsume();
            });
        }

        if (TryApplyItemEffect(state.Definition.Type) == false)
        {
            return false;
        }

        if (itemType != ItemType.Undo)
        {
            ClearUndoHistory();
        }

        return state.TryConsume();
    }

    private bool TryApplyItemEffect(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.MoveToTempBoard:
                return TryMoveFirstTilesToTempBoard();
            case ItemType.Hammer:
                return TryUseHammer();
            case ItemType.Shuffle:
                return TryUseShuffle();
            case ItemType.Undo:
                return _trayController.TryUndoLastMove();
            // 따로 처리
            //case ItemType.FreeSelect:
            default:
                Debug.Log($"아직 구현되지 않은 아이템: {itemType}");
                return false;
        }
    }

    private bool TryMoveFirstTilesToTempBoard()
    {
        var tiles = _trayController.TakeFirst(3);
        if (tiles.Count == 0)
        {
            return false;
        }

        _tempBoardController.StackTiles(tiles);
        return true;
    }

    private void OnDisable()
    {
        _boardController.StateChanged -= HandleStateChanged;
        _trayController.StateChanged -= HandleStateChanged;
        _tempBoardController.StateChanged -= HandleStateChanged;
        _trayController.TilesRemoved -= HandleTilesRemoved;
    }
}
