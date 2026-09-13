using System;
using UnityEngine;

public class PlayerScreenController : MonoBehaviour
{
    [SerializeField]
    private PlayerController _localPlayer;
    [SerializeField]
    private ItemBarController _ItemBar;
    [SerializeField]
    private CanvasGroup _itemBarCanvasGroup;
    [SerializeField]
    private PlayerScreenBinding[] _bindings;

    private void OnEnable()
    {
        foreach (var binding in _bindings)
        {
            binding.Profile.Clicked += HandleProfileClicked;
        }
    }

    private void Start()
    {
        ShowPlayer(_localPlayer);
    }

    private void SetScreenState(PlayerScreenBinding binding, bool isSelected)
    {
        bool allowInput = isSelected && binding.Player == _localPlayer;

        binding.Screen.alpha = isSelected ? 1f : 0f;
        binding.Screen.interactable = allowInput;
        binding.Screen.blocksRaycasts = allowInput;
    }

    private void ShowPlayer(PlayerController player)
    {
        PlayerScreenBinding selectedBinding = Array.Find(_bindings, x => x.Player == player);

        if (selectedBinding == null)
        {
            Debug.LogError("선택한 플레이어의 화면 연결 정보가 없습니다.");
            return;
        }

        foreach (var binding in _bindings)
        {
            SetScreenState(binding, binding == selectedBinding);
        }

        _ItemBar.Bind(player);

        bool isLocalPlayer = player == _localPlayer;

        _itemBarCanvasGroup.interactable = isLocalPlayer;
        _itemBarCanvasGroup.blocksRaycasts = isLocalPlayer;

        Debug.Log($"표시 플레이어 변경: {player.name}");
    }

    private void HandleProfileClicked(PlayerController player)
    {
        ShowPlayer(player);
    }

    private void OnDisable()
    {
        foreach (var binding in _bindings)
        {
            binding.Profile.Clicked -= HandleProfileClicked;
        }
    }
}
