using TMPro;
using UnityEngine;

public class OrderView : MonoBehaviour
{
    [SerializeField]
    private OrderController _orderController;
    [SerializeField]
    private TMP_Text _requiredCountText;
    [SerializeField]
    private TMP_Text _progressText;

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
