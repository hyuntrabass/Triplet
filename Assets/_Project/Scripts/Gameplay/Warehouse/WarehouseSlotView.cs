using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WarehouseSlotController), typeof(Button))]
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
    private Button _button;

    public RectTransform TileAnchor => _tileAnchor;
    public PlayerController Owner => _controller.Owner;
    public WarehouseSlotState State => _controller.State;

    public bool CanAdd => _isOwnerContext && _controller.State.Status == WarehouseSlotStatus.Ready;

    public event Action<WarehouseSlotView> Clicked;

    private void Awake()
    {
        _controller = GetComponent<WarehouseSlotController>();
        _button = GetComponent<Button>();

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
        _button.onClick.AddListener(HandleClick);
    }

    private void Start()
    {
        RefreshView();
    }

    private void HandleClick()
    {
        Clicked?.Invoke(this);
    }

    private void HandleStateChanged(WarehouseSlotController controller)
    {
        RefreshView();
    }

    private void RefreshView()
    {
        WarehouseSlotState state = _controller.State;

        float chargeRatio = (float)state.Charge / WarehouseSlotState.RequiredCharge;

        _shutterImage.fillAmount = 1f - chargeRatio;

        bool showPlus = _isOwnerContext && state.Status != WarehouseSlotStatus.Occupied;
        _plusObject.SetActive(showPlus);

        bool canTake = state.Status == WarehouseSlotStatus.Occupied;
        _button.interactable = CanAdd || canTake;
    }

    private void OnDisable()
    {
        _controller.StateChanged -= HandleStateChanged;
        _button.onClick.RemoveListener(HandleClick);
    }
}
