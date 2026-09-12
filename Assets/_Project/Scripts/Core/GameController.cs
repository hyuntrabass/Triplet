using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerController[] _players;
    private bool _isFinished;
    private bool _needEvaluation;

    private void OnEnable()
    {
        foreach (var item in _players)
        {
            item.StateChanged += HandleStateChanged;
        }
    }

    private void Start()
    {
        _needEvaluation = true;
    }

    private void HandleStateChanged()
    {
        _needEvaluation = true;
    }

    private void LateUpdate()
    {
        if (_isFinished || _needEvaluation == false)
        {
            return;
        }

        _needEvaluation = false;
        EvaluateGameState();
    }

    private void EvaluateGameState()
    {
        if (_players.Length == 0)
        {
            return;
        }

        if (_players.Any(x => x.IsClear))
        {
            _isFinished = true;
            Debug.Log("Game Clear");
            return;
        }

        if (_players.All(x => x.IsDown))
        {
            _isFinished = true;
            Debug.Log("Game Over");
        }
    }

    private void OnDisable()
    {
        foreach (var item in _players)
        {
            item.StateChanged -= HandleStateChanged;
        }
    }
}
