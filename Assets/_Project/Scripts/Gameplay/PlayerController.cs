using System;
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
        _boardController.SetInputEnabled(!IsDown);

        if (_downIndicator != null)
        {
            _downIndicator.SetActive(IsDown);
        }
    }

    public void ClearMainTray()
    {
        _trayController.ClearAll();
    }

    public void MoveFirstTilesToTempBoard()
    {
        var tiles = _trayController.TakeFirst(3);
        _tempBoardController.StackTiles(tiles);
    }

    private void OnDisable()
    {
        _boardController.StateChanged -= HandleStateChanged;
        _trayController.StateChanged -= HandleStateChanged;
        _tempBoardController.StateChanged -= HandleStateChanged;
    }
}
