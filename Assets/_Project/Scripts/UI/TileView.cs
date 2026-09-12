using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class TileView : MonoBehaviour
{
    [SerializeField] 
    private int _typeId;
    private Button _button;
    [SerializeField] 
    private Color[] _typeColors;
    private Image _image;

    public int TypeId => _typeId;
    public int StackLevel { get; private set; }

    public event Action<TileView> Clicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
        _image = GetComponent<Image>();
    }

    public void Init(int typeId, int stackLevel)
    {
        _typeId = typeId;
        StackLevel = stackLevel;

        if (_typeColors.Length > 0)
        {
            int colorIndex = (typeId - 1) % _typeColors.Length;
            _image.color = _typeColors[colorIndex];
        }
    }

    public void SetInteractable(bool value)
    {
        _button.interactable = value;
    }

    private void HandleClick()
    {
        Debug.Log($"타일클릭: {_typeId}");

        Clicked?.Invoke(this);
    }
}
