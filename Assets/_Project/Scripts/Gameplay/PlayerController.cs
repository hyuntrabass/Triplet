using System;
using System.Collections.Generic;
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
    private GameObject _downIndicator;
    [SerializeField]
    private ItemDefinition[] _itemLoadout;

    private ItemInventory _itemInventory;

    public bool IsDown => _trayController.IsFull;
    public bool IsClear => _boardController.IsEmpty && _tempBoardController.IsEmpty;
    public IReadOnlyList<ItemState> ItemStates => _itemInventory.States;

    public event Action StateChanged;
    public event Action<int, int> TilesRemoved;

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

    void Start()
    {
        RefreshPlayerState();
    }

    public ItemState GrantRandomItem()
    {
        return _itemInventory.GrantRandomItem();
    }

    private void HandleStateChanged()
    {
        RefreshPlayerState();
        StateChanged?.Invoke();
    }

    private void HandleTilesRemoved(int typeId, int count)
    {
        TilesRemoved?.Invoke(typeId, count);
    }

    private void RefreshPlayerState()
    {
        if (_downIndicator != null)
        {
            _downIndicator.SetActive(IsDown);
        }
    }

    public void ClearMainTray()
    {
        _trayController.ClearAll();
    }

    public bool TryUseItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.MoveToTempBoard:
                return TryMoveFirstTilesToTempBoard();
            case ItemType.Undo:
            case ItemType.Shuffle:
            case ItemType.Hammer:
            case ItemType.PickBack:
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
