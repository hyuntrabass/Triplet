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

    private ItemInventory _itemInventory;

    public bool IsDown => _trayController.IsFull;
    public bool IsClear => _boardController.IsEmpty && _tempBoardController.IsEmpty;
    public IReadOnlyList<ItemState> ItemStates => _itemInventory.States;
    public float ClearProgress => _boardController.ClearProgress;

    public event Action StateChanged;
    public event Action<int, int> OrderTilesRemoved;
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
        OrderTilesRemoved?.Invoke(typeId, count);
        TilesMatched?.Invoke(typeId, count);
    }

    //private void HandlePickBackSelected(ItemState state, TileView tile)
    //{
    //    if (_trayController.TryAdd(tile) == false)
    //    {
    //        Debug.Log("수납함이 가득 찼습니다.");
    //        return;
    //    }

    //    if (_boardController.TryDetachTile(tile) == false)
    //    {
    //        throw new InvalidOperationException("선택한 타일을 보드에서 분리하지 못했습니다.");
    //    }

    //    if (state.TryConsume() == false)
    //    {
    //        throw new InvalidOperationException("PickBack 아이템을 소모하지 못했습니다.");
    //    }
    //}

    public IReadOnlyList<TileView> GetWarehouseCandidates()
    {
        var candidates = new List<TileView>();

        candidates.AddRange(_boardController.GetExposedTiles());
        candidates.AddRange(_trayController.GetTiles());
        candidates.AddRange(_tempBoardController.GetTopTiles());

        return candidates;
    }

    //private bool TryBeginPickBack(ItemState state)
    //{
    //    if (_trayController.IsFull)
    //    {
    //        return false;
    //    }

    //    IReadOnlyList<TileView> candidates = _boardController.GetAllTiles();

    //    return _tileSelectionController.TryBegin(candidates, "가져올 타일을 선택하세요", x => HandlePickBackSelected(state, x));
    //}

    private bool TryUseHammer()
    {
        IReadOnlyList<int> removedTypeIds = _boardController.RemoveExposedTiles();

        if (removedTypeIds.Count == 0)
        {
            return false;
        }

        foreach (var group in removedTypeIds.GroupBy(x => x))
        {
            OrderTilesRemoved?.Invoke(group.Key, group.Count());
        }

        return true;
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

        if (TryApplyItemEffect(state.Definition.Type) == false)
        {
            return false;
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
            case ItemType.Undo:
            case ItemType.Shuffle:
            case ItemType.FreeSelect:
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
