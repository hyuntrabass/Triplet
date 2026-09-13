using System;
using UnityEngine;

public class OrderController : MonoBehaviour
{
    [SerializeField]
    private PlayerController[] _players;
    [SerializeField]
    private ItemBarController[] _itemBars;
    [SerializeField]
    private int _targetTypeId = 1;
    [SerializeField, Min(1)]
    private int _shortOrderStartAfterCompletions = 5;

    private int _requiredCount;
    private int _completedOrderCount;
    private int _currentCount;

    public int TargetTypeId => _targetTypeId;
    public int RequiredCount => _requiredCount;
    public int CompletedOrderCount => _completedOrderCount;
    public int CurrentCount => _currentCount;

    public event Action StateChanged;

    private void OnEnable()
    {
        foreach (var player in _players)
        {
            player.TilesRemoved += HandleTilesRemoved;
        }
    }

    private void Start()
    {
        _requiredCount = SelectNextRequiredCount();
        StateChanged?.Invoke();
    }

    private void CompleteOrder()
    {
        foreach (var itemBar in _itemBars)
        {
            itemBar.GrantRandomItem();
        }

        Debug.Log($"주문 완료: TypeId {_targetTypeId}");

        _completedOrderCount++;
        _currentCount = 0;
        _requiredCount = SelectNextRequiredCount();

        StateChanged?.Invoke();
    }

    private int SelectNextRequiredCount()
    {
        if (_completedOrderCount >= _shortOrderStartAfterCompletions)
        {
            return 3;
        }

        return UnityEngine.Random.Range(0, 2) == 0 ? 9 : 12;
    }

    private void HandleTilesRemoved(int typeId, int count)
    {
        if (typeId != _targetTypeId || count <= 0)
        {
            return;
        }

        _currentCount = Mathf.Min(_currentCount + count, _requiredCount);

        Debug.Log($"주문 진행 {_currentCount}/{_requiredCount}");

        if (_currentCount >= _requiredCount)
        {
            CompleteOrder();
            return;
        }

        StateChanged?.Invoke();
    }

    private void OnDisable()
    {
        foreach (var player in _players)
        {
            player.TilesRemoved -= HandleTilesRemoved;
        }
    }
}
