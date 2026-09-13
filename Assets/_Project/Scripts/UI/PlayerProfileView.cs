using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileView : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;
    [SerializeField]
    private GameObject _sosIndicator;
    [SerializeField]
    private TMP_Text _progressText;

    private Button _button;

    public event Action<PlayerController> Clicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void OnEnable()
    {
        _player.StateChanged += HandleStateChanged;
    }

    private void Start()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        int progressPercent = Mathf.RoundToInt(_player.ClearProgress * 100f);

        _progressText.text = $"{progressPercent}%";
        _sosIndicator.SetActive(_player.IsDown);
    }

    private void HandleClick()
    {
        Clicked?.Invoke(_player);
    }

    private void HandleStateChanged()
    {
        RefreshView();
    }

    private void OnDisable()
    {
        _player.StateChanged -= HandleStateChanged;
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }
}
