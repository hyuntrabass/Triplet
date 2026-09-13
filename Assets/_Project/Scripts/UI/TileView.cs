using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class TileView : MonoBehaviour
{
    private Button _button;
    private Image _image;
    private TileDefinition _definition;

    public int TypeId => _definition.TypeId;
    public int StackLevel { get; private set; }
    public bool IsInteractable => _button.interactable;

    private Action<TileView> _selectionHandler;

    public event Action<TileView> Clicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
        _image = GetComponent<Image>();
    }

    public void Init(TileDefinition definition, int stackLevel)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        _definition = definition;
        StackLevel = stackLevel;

        if (_definition.Sprite != null)
        {
            _image.sprite = _definition.Sprite;
            _image.color = Color.white;
        }
        else
        {
            _image.sprite = null;
            _image.color = _definition.PlaceholderColor;
        }
    }

    public void BeginSelection(Action<TileView> selectionHandler)
    {
        if (selectionHandler == null)
        {
            throw new ArgumentNullException(nameof(selectionHandler));
        }

        if (_selectionHandler != null)
        {
            throw new InvalidOperationException($"{name}은 이미 선택 모드입니다.");
        }

        _selectionHandler = selectionHandler;
        SetInteractable(true);
    }

    public void EndSelection()
    {
        _selectionHandler = null;
    }

    public void SetInteractable(bool value)
    {
        _button.interactable = value;
        _image.raycastTarget = value;
    }

    private void HandleClick()
    {
        if (_selectionHandler != null)
        {
            _selectionHandler.Invoke(this);
            return;
        }

        Debug.Log($"타일클릭: {TypeId}");
        Clicked?.Invoke(this);
    }
}
