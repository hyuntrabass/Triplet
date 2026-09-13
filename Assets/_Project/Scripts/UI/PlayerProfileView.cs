using TMPro;
using UnityEngine;

public class PlayerProfileView : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;
    [SerializeField]
    private GameObject _sosIndicator;
    [SerializeField]
    private TMP_Text _progressText;

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

    private void HandleStateChanged()
    {
        RefreshView();
    }

    private void OnDisable()
    {
        _player.StateChanged -= HandleStateChanged;
    }
}
