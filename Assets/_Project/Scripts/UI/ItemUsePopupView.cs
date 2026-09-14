using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsePopupView : MonoBehaviour
{
    [SerializeField]
    private GameObject _popupRoot;
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private TMP_Text _descriptionText;
    [SerializeField]
    private Button _useButton;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private TMP_Text _emptyMessageText;

    private ItemState _state;

    public event Action<ItemState> UseRequested;

    private void Awake()
    {
        if (_popupRoot == null ||
            _titleText == null ||
            _iconImage == null ||
            _descriptionText == null ||
            _useButton == null ||
            _emptyMessageText == null ||
            _closeButton == null)
        {
            throw new InvalidOperationException("아이템 사용 팝업의 UI 참조가 연결되지 않았습니다.");
        }

        _useButton.onClick.AddListener(HandleUseClicked);
        _closeButton.onClick.AddListener(Close);

        _popupRoot.SetActive(false);
    }

    public void Open(ItemState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));

        ItemDefinition definition = state.Definition;

        _titleText.text = definition.DisplayName;
        _descriptionText.text = definition.Description;

        _iconImage.sprite = definition.Icon;
        _iconImage.enabled = definition.Icon != null;

        bool canUse = state.CanUse;
        _useButton.interactable = canUse;
        _emptyMessageText.gameObject.SetActive(canUse);

        _popupRoot.SetActive(true);
    }

    public void Close()
    {
        _state = null;
        _popupRoot.SetActive(false);
    }

    private void HandleUseClicked()
    {
        if (_state == null || _state.CanUse == false)
        {
            return;
        }

        UseRequested?.Invoke(_state);
    }

    private void OnDestroy()
    {
        _useButton.onClick.RemoveListener(HandleUseClicked);
        _closeButton.onClick.RemoveListener(Close);
    }
}
