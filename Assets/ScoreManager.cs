using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    // Specjalny słownik Mirrora - automatycznie synchronizuje się ze wszystkimi graczami!
    // Klucz: Nazwa gracza, Wartość: Liczba punktów (owiec)
    public readonly SyncDictionary<string, int> playerScores = new SyncDictionary<string, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void OnStartClient()
    {
        // Gdy klient dołącza, podpinamy funkcję, która odświeży UI przy każdej zmianie punktów
        playerScores.OnChange += OnScoresChanged;
    }

    // Ta funkcja odpala się u KAŻDEGO gracza, gdy ktoś zdobędzie lub straci punkt
    private void OnScoresChanged(SyncDictionary<string, int>.Operation op, string key, int item)
    {
        if (ScoreboardUI.Instance != null)
        {
            ScoreboardUI.Instance.UpdateScoreboard(playerScores);
        }
    }

    [Server]
    public void ChangeScore(string playerName, int pointsChange)
    {
        if (string.IsNullOrEmpty(playerName)) return;

        // Jeśli gracza nie ma jeszcze w tabeli, dodaj go z 0 punktów
        if (!playerScores.ContainsKey(playerName))
        {
            playerScores[playerName] = 0;
        }

        // Dodaj lub odejmij punkty
        playerScores[playerName] += pointsChange;

        // Opcjonalne zabezpieczenie: żeby punkty nie zeszły poniżej zera
        if (playerScores[playerName] < 0)
        {
            playerScores[playerName] = 0;
        }
    }
}