using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemButtonView : MonoBehaviour
{
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private TMP_Text _countText;

    private Button _button;
    private ItemState _state;

    public event Action<ItemState> Clicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    public void Init(ItemState state)
    {
        if (_state != null)
        {
            _state.Changed -= RefreshView;
        }

        _state = state
            ?? throw new ArgumentNullException(nameof(state));

        _state.Changed += RefreshView;

        _iconImage.sprite = state.Definition.Icon;
        _iconImage.enabled = state.Definition.Icon != null;

        RefreshView();
    }

    public void RefreshView()
    {
        if (_state == null)
        {
            return;
        }

        _countText.text = _state.Count.ToString();
        _button.interactable = _state.CanUse;
    }

    private void HandleClick()
    {
        if (_state == null || _state.CanUse == false)
        {
            return;
        }

        Clicked?.Invoke(_state);
    }

    private void OnDestroy()
    {
        if (_state != null)
        {
            _state.Changed -= RefreshView;
        }
        
        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }
}
