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
        scoreboardText.text = "<b>WYNIKI:</b>\n<i>Czekam na graczy...</i>";
    }

    // Wywoływane automatycznie przez ScoreManager
    public void UpdateScoreboard(IDictionary<string, int> scores)
    {
        string newText = "<b>WYNIKI:</b>\n";

        foreach (var score in scores)
        {
            // Budujemy linijkę, np: "Gracz1: 5 owiec"
            newText += $"{score.Key}: {score.Value}\n";
        }

        scoreboardText.text = newText;
    }
}