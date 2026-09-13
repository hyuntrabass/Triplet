using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBarController : MonoBehaviour
{
    [SerializeField]
    private ItemButtonView[] _buttons;
    [SerializeField]
    private PlayerController _playerController;

    private void Start()
    {
        Bind(_playerController);
    }

    public void Bind(PlayerController player)
    {
        if (player == null)
        {
            throw new System.ArgumentNullException(nameof(player));
        }

        if (_buttons.Length != player.ItemStates.Count)
        {
            Debug.LogError($"아이템 버튼 수({_buttons.Length})와 아이템 상태 수({player.ItemStates.Count})가 다릅니다.");
            return;
        }

        UnsubscribeButtons();

        _playerController = player;

        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].Init(player.ItemStates[i]);
            _buttons[i].Clicked += HandleItemClicked;
        }
    }

    private void UnsubscribeButtons()
    {
        foreach (ItemButtonView button in _buttons)
        {
            if (button != null)
            {
                button.Clicked -= HandleItemClicked;
            }
        }
    }

    private void HandleItemClicked(ItemState state)
    {
        _playerController.TryUseItem(state);
    }

    private void OnDestroy()
    {
        UnsubscribeButtons();
    }
}
