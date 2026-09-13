using System;
using UnityEngine;

public class WarehouseSlotController : MonoBehaviour
{
    [SerializeField]
    private PlayerController _owner;

    private WarehouseSlotState _state;

    public PlayerController Owner => _owner;
    public WarehouseSlotState State => _state;

    public event Action<WarehouseSlotController> StateChanged;

    private void Awake()
    {
        if (_owner == null)
        {
            throw new InvalidOperationException($"{name}에 창고 소유자가 연결되지 않았습니다.");
        }

        _state = new WarehouseSlotState(_owner);
    }

    private void OnEnable()
    {
        _owner.TilesRemoved += HandleTilesRemoved;
        _state.Changed += HandleSlotStateChanged;
    }

    private void HandleTilesRemoved(int typeId, int removedTileCount)
    {
        _state.AddMatchedTiles(removedTileCount);
    }

    private void HandleSlotStateChanged()
    {
        Debug.Log(
            $"{_owner.name} 창고 상태: {_state.Status}, " +
            $"충전: {_state.Charge}/{WarehouseSlotState.RequiredCharge}");

        StateChanged?.Invoke(this);
    }

    private void OnDisable()
    {
        _owner.TilesRemoved -= HandleTilesRemoved;
        _state.Changed -= HandleSlotStateChanged;
    }
}
