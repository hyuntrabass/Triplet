using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor.EditorTools;

[RequireComponent(typeof(WarehouseSlotController))]
public class WarehouseSlotView : MonoBehaviour
{
    [SerializeField]
    private Image _shutterImage;
    [SerializeField]
    private GameObject _plusObject;
    [SerializeField]
    private RectTransform _tileAnchor;

    private WarehouseSlotController  _controller;
    private bool _isOwnerContext;

    public RectTransform TileAnchor => _tileAnchor;
    public PlayerController Owner => _controller.Owner;

    public bool CanAdd => _isOwnerContext && _controller.State.Status == WarehouseSlotStatus.Ready;

    private void Awake()
    {
        _controller = GetComponent<WarehouseSlotController>();

        if (_shutterImage == null ||
            _plusObject == null ||
            _tileAnchor == null)
        {
            throw new InvalidOperationException($"{name}의 창고 UI 참조가 연결되지 않았습니다.");
        }
    }

    public void SetOwnerContext(bool value)
    {
        if (_isOwnerContext == value)
        {
            return;
        }

        _isOwnerContext = value;
        RefreshView();
    }

    private void OnEnable()
    {
        _controller.StateChanged += HandleStateChanged;
    }

    private void Start()
    {
        RefreshView();
    }

    private void HandleStateChanged(WarehouseSlotController controller)
    {
        RefreshView();
    }

    private void RefreshView()
    {
        WarehouseSlotState state = _controller.State;

        float ChargeRatio = (float)state.Charge / WarehouseSlotState.RequiredCharge;

        _shutterImage.fillAmount = 1f - ChargeRatio;

        bool showPlus = _isOwnerContext && state.Status != WarehouseSlotStatus.Occupied;

        _plusObject.SetActive(showPlus);
    }

    private void OnDisable()
    {
        _controller.StateChanged -= HandleStateChanged;
    }
}
