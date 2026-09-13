using System;
using UnityEngine;

public class OrderController : MonoBehaviour
{
    [SerializeField]
    private PlayerController[] _players;
    [SerializeField]
    private TileDefinition[] _targetCandidates;
    [SerializeField, Min(1)]
    private int _shortOrderStartAfterCompletions = 5;

    private int _requiredCount;
    private int _completedOrderCount;
    private int _currentCount;
    private TileDefinition _target;

    public TileDefinition Target => _target;
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
        _target = SelectNextTarget();
        _requiredCount = SelectNextRequiredCount();

        StateChanged?.Invoke();
    }

    private TileDefinition SelectNextTarget()
    {
        if (_targetCandidates == null || _targetCandidates.Length == 0)
        {
            throw new InvalidOperationException("주문 대상 TileDefinition이 설정되지 않았습니다.");
        }

        int index = UnityEngine.Random.Range(0, _targetCandidates.Length);
        TileDefinition target = _targetCandidates[index];

        if (target == null)
        {
            throw new InvalidOperationException($"주문 대상 배열의 Element {index}가 비어 있습니다.");
        }

        return target;
    }

    private void CompleteOrder()
    {
        foreach (var player in _players)
        {
            player.GrantRandomItem();
        }

        Debug.Log($"주문 완료: TypeId {_target.TypeId}");

        _completedOrderCount++;
        _currentCount = 0;
        _target = SelectNextTarget();
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
        if (typeId != _target.TypeId || count <= 0)
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
