using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerController[] _players;
    [SerializeField]
    private GameResultView _resultView;

    private bool _isFinished;
    private bool _needEvaluation;
    private bool _isRestarting;

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
            FinishGame(true);
            return;
        }

        if (_players.All(x => x.IsDown))
        {
            FinishGame(false);
        }
    }

    private void FinishGame(bool isWin)
    {
        if (_isFinished)
        {
            return;
        }

        _isFinished = true;

        foreach (var player in _players)
        {
            player.Statistics.Freeze();
        }

        _resultView.Show(isWin, _players);

        Debug.Log(isWin ? "GameClear" : "Game Over");
    }

    public void RestartGame()
    {
        if (_isFinished == false || _isRestarting)
        {
            return;
        }

        _isRestarting = true;

        SceneManager.LoadSceneAsync(gameObject.scene.path, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        foreach (var item in _players)
        {
            item.StateChanged -= HandleStateChanged;
        }
    }
}
