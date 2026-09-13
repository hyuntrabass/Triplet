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
    [SerializeField]
    private TileSelectionController _tileSelectionController;

    private void Awake()
    {
        if (_localPlayer == null 
            || _screenController == null 
            || _tileSelectionController == null 
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
            BeginStoreSelection(slotView);
            return;
        }

        if (slotView.State.Status == WarehouseSlotStatus.Occupied)
        {
            Debug.Log($"{slotView.Owner.name}의 창고 타일 가져가기");
        }
    }

    private void HandleWarehouseTileSelected(WarehouseSlotView slotView, TileView tile)
    {
        if (slotView.CanAdd == false)
        {
            return;
        }

        if (_localPlayer.TryDetachTile(tile) == false)
        {
            Debug.LogWarning("선택한 타일을 기존 위치에서 분리하지 못했습니다.");
            return;
        }

        if (slotView.TryStore(tile) == false)
        {
            throw new InvalidOperationException("타일을 분리한 뒤 공용창고 저장에 실패했습니다.");
        }

        RectTransform tileRect = (RectTransform)tile.transform;

        tileRect.SetParent(slotView.TileAnchor, false);
        tileRect.anchoredPosition = Vector2.zero;
        tileRect.localRotation = Quaternion.identity;
        tileRect.localScale = Vector3.one;

        tile.SetInteractable(false);
    }

    private void BeginStoreSelection(WarehouseSlotView slotView)
    {
        var candidates = _localPlayer.GetWarehouseCandidates();

        bool started = _tileSelectionController.TryBegin(candidates, "[공공창고]에 추가할 카드를 선택하세요", x => HandleWarehouseTileSelected(slotView, x));

        if (started == false)
        {
            Debug.Log("공용창고에 올릴 수 있는 타일이 없습니다.");
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
