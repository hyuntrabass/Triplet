using System;
using UnityEngine;

[Serializable]
public class PlayerScreenBinding
{
    [SerializeField]
    private PlayerController _player;
    [SerializeField]
    private PlayerProfileView _profile;
    [SerializeField]
    private CanvasGroup _screen;

    public PlayerController Player => _player;
    public PlayerProfileView Profile => _profile;
    public CanvasGroup Screen => _screen;
}
