using System;
using UnityEngine;

public class SharedWarehouseController : MonoBehaviour
{
    [SerializeField]
    private PlayerController _localPlayer;
    [SerializeField]
    private PlayerScreenController _screenController;
    [SerializeField]
    private WarehouseSlotView[] _slotViews;

    private void Awake()
    {
        if (_localPlayer == null 
            || _screenController == null 
            || _slotViews == null 
            || _slotViews.Length != 3)
        {
            throw new InvalidOperationException("공용창고 참조가 올바르게 연결되지 않았습니다.");
        }
    }

    private void OnEnable()
    {
        _screenController.DisplayedPlayerChanged += HandleDisplayedPlayerChanged;

        foreach (var item in _slotViews)
        {
            item.Clicked += HandleSlotClicked;
        }
    }

    private void Start()
    {
        RefreshOwnerContext(_screenController.DisplayedPlayer);
    }

    private void HandleDisplayedPlayerChanged(PlayerController displayedPlayer)
    {
        RefreshOwnerContext(displayedPlayer);
    }

    private void HandleSlotClicked(WarehouseSlotView slotView)
    {
        if (slotView.CanAdd)
        {
            Debug.Log($"{slotView.Owner.name}의 창고에 넣을 타일 선택 시작");
            return;
        }

        if (slotView.State.Status == WarehouseSlotStatus.Occupied)
        {
            Debug.Log($"{slotView.Owner.name}의 창고 타일 가져가기");
        }
    }

    private void RefreshOwnerContext(PlayerController displayedPlayer)
    {
        bool isViewingLocalPlayer = displayedPlayer == _localPlayer;

        foreach (var slotView in _slotViews)
        {
            bool isLocalPlayerSlot = slotView.Owner == _localPlayer;

            slotView.SetOwnerContext(isViewingLocalPlayer && isLocalPlayerSlot);
        }
    }

    private void OnDisable()
    {
        _screenController.DisplayedPlayerChanged -= HandleDisplayedPlayerChanged;

        foreach (var item in _slotViews)
        {
            item.Clicked -= HandleSlotClicked;
        }
    }
}
