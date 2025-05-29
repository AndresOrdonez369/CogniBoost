using UnityEngine;
using TMPro; // Para TextMeshProUGUI

public class LeaderboardRowUI : MonoBehaviour
{
    [Tooltip("Texto para mostrar el rango (ej. 1., 2.)")]
    public TextMeshProUGUI rankText;

    [Tooltip("Texto para mostrar el nombre del jugador")]
    public TextMeshProUGUI nameText;

    [Tooltip("Texto para mostrar el puntaje del jugador")]
    public TextMeshProUGUI scoreText;


    /// <summary>
    /// Configura los textos de esta fila del leaderboard.
    /// </summary>
    /// <param name="rank">La posición en el leaderboard.</param>
    /// <param name="playerName">El nombre del jugador.</param>
    /// <param name="score">El puntaje.</param>
    public void Setup(int rank, string playerName, int score)
    {
        if (rankText != null)
        {
            rankText.text = $"{rank}.";
        }
        else
        {
            Debug.LogWarning("RankText no asignado en LeaderboardRowUI", this.gameObject);
        }

        if (nameText != null)
        {
            nameText.text = playerName;
        }
        else
        {
            Debug.LogWarning("NameText no asignado en LeaderboardRowUI", this.gameObject);
        }

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreTexxt no asignado en LeaderboardRowUI", this.gameObject);
        }
    }
}