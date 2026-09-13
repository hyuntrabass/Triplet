using UnityEngine;

public class PlayerProfileView : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;
    [SerializeField]
    private GameObject _sosIndicator;

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
