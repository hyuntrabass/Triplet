using System.Text;
using TMPro;
using UnityEngine;

public class GameResultView : MonoBehaviour
{
    [SerializeField]
    private GameObject _root;
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private TMP_Text _statisticsText;

    private void Awake()
    {
        _titleText.raycastTarget = false;
        _statisticsText.raycastTarget = false;

        _root.SetActive(false);
    }

    public void Show(bool isWin, PlayerController[] players)
    {
        int totalContribution = 0;

        foreach (var player in players)
        {
            totalContribution += player.Statistics.OrderContribution;
        }

        var text = new StringBuilder();

        for (int i = 0; i < players.Length; i++)
        {
            var player = players[i];
            var statistics = player.Statistics;

            float contributionRate = totalContribution > 0 ? statistics.OrderContribution * 100f / totalContribution : 0f;

            text.AppendLine($"{i + 1}. {player.DisplayName}");

            text.AppendLine(
                $"주문 기여: {statistics.OrderContribution}개 " +
                $"({contributionRate:0.#}%)");

            text.AppendLine(
                $"창고에 올림: {statistics.WarehouseStoredCount}개");

            text.AppendLine(
                $"낼름: {statistics.WarehouseTakenCount}개");

            text.AppendLine(
                $"남의 것 낼름 {statistics.OthersWarehouseTakenCount}개 / " +
                $"내 것 낼름 {statistics.OwnWarehouseTakenCount}개");

            text.AppendLine();
        }

        _titleText.text = isWin ? "게임 승리!" : "도전 실패";
        _statisticsText.text = text.ToString();

        transform.SetAsLastSibling();
        _root.SetActive(true);
    }
}
