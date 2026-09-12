using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private BoardController _boardController;
    [SerializeField]
    private TrayController _trayController;
    [SerializeField]
    private GameObject _downIndicator;

    public bool IsDown => _trayController.IsFull;
    public bool IsClear => _boardController.IsEmpty;

    public event Action StateChanged;

    private void OnEnable()
    {
        _boardController.StateChanged += HandleStateChanged;
        _trayController.StateChanged += HandleStateChanged;
    }

    void Start()
    {

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

    private void OnDisable()
    {
        _boardController.StateChanged -= HandleStateChanged;
        _trayController.StateChanged -= HandleStateChanged;
    }
}
