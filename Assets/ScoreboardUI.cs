using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreboardUI : MonoBehaviour
{
    public static ScoreboardUI Instance;

    [Header("Przypisz pole tekstowe z Canvasa")]
    public TextMeshProUGUI scoreboardText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Zamiast na sztywno wpisywać "Czekam na graczy", najpierw sprawdzamy, 
        // czy serwer zdążył nam już przysłać jakieś zera!
        if (ScoreManager.Instance != null && ScoreManager.Instance.playerScores.Count > 0)
        {
            UpdateScoreboard(ScoreManager.Instance.playerScores);
        }
        else
        {
            scoreboardText.text = "<b>WYNIKI:</b>\n<i>Czekam na graczy...</i>";
        }
    }

    // Wywoływane automatycznie przez ScoreManager
    public void UpdateScoreboard(IDictionary<string, int> scores)
    {
        string newText = "<b>WYNIKI:</b>\n";

        foreach (var score in scores)
        {
            newText += $"{score.Key}: {score.Value}\n";
        }

        // Zabezpieczenie, gdyby mapa faktycznie była jeszcze całkowicie pusta
        if (scores.Count == 0)
        {
            newText += "<i>Czekam na graczy...</i>";
        }

        scoreboardText.text = newText;
    }
}