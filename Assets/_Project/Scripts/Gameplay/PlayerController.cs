using System;
using Triplet.Core;
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

    public bool IsDown => _trayController.IsFull;
    public bool IsClear => _boardController.IsEmpty && _tempBoardController.IsEmpty;

    public event Action StateChanged;

    private void OnEnable()
    {
        _boardController.StateChanged += HandleStateChanged;
        _trayController.StateChanged += HandleStateChanged;
        _tempBoardController.StateChanged += HandleStateChanged;
    }

    void Start()
    {
        RefreshPlayerState();
    }

    private void HandleStateChanged()
    {
        RefreshPlayerState();
        StateChanged?.Invoke();
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
    }
}
