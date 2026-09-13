using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderView : MonoBehaviour
{
    [SerializeField]
    private OrderController _orderController;
    [SerializeField]
    private TMP_Text _requiredCountText;
    [SerializeField]
    private TMP_Text _progressText;
    [SerializeField]
    private Image _targetIcon;

    private void OnEnable()
    {
        _orderController.StateChanged += HandleStateChanged;
    }

    private void Start()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        TileDefinition target = _orderController.Target;

        if (target == null)
        {
            return;
        }

        if (target.Sprite != null)
        {
            _targetIcon.sprite = target.Sprite;
            _targetIcon.color = Color.white;
        }
        else
        {
            _targetIcon.sprite = null;
            _targetIcon.color = target.PlaceholderColor;
        }

        _requiredCountText.text = $"x {_orderController.RequiredCount}";
        _progressText.text = $"{_orderController.CurrentCount} / {_orderController.RequiredCount}";
    }

    private void HandleStateChanged()
    {
        RefreshView();
    }

    private void OnDisable()
    {
        _orderController.StateChanged -= HandleStateChanged;
    }
}
