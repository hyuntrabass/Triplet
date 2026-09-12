using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBarController : MonoBehaviour
{
    [SerializeField]
    private ItemButtonView[] _buttons;
    [SerializeField]
    private ItemDefinition[] _loadout;
    [SerializeField]
    private PlayerController _playerController;

    private readonly List<ItemState> _states = new();

    private void Start()
    {
        if (_buttons.Length != _loadout.Length)
        {
            Debug.LogError("아이템 버튼과 아이템 정의의 개수가 다릅니다.", this);

            enabled = false;
            return;
        }

        for (int i = 0; i < _loadout.Length; i++)
        {
            var state = new ItemState(_loadout[i]);

            _states.Add(state);

            _buttons[i].Init(state);
            _buttons[i].Clicked += HandleItemClicked;
        }
    }

    [ContextMenu("Grant Random Item")]
    public void GrantRandomItem()
    {
        if (_states.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, _states.Count);
        _states[index].Add();
    }

    private void HandleItemClicked(ItemState state)
    {
        bool succeeded = _playerController.TryUseItem(state.Definition.Type);

        if (succeeded == false)
        {
            return;
        }

        state.TryConsume();
    }

    private void OnDestroy()
    {
        foreach (var item in _buttons)
        {
            if (item != null)
            {
                item.Clicked -= HandleItemClicked;
            }
        }
    }
}
