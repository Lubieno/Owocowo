using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    // Specjalny słownik Mirrora - automatycznie synchronizuje się ze wszystkimi graczami!
    public readonly SyncDictionary<string, int> playerScores = new SyncDictionary<string, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void OnStartServer()
    {
        // Serwer co 0.5 sekundy będzie sprawdzał, czy jest jakiś gracz, który nie ma wpisanego "0"
        InvokeRepeating(nameof(RegisterMissingPlayers), 0.5f, 1f);
    }

    public override void OnStartClient()
    {
        // Gdy klient dołącza, podpinamy funkcję aktualizującą UI
        playerScores.OnChange += OnScoresChanged;

        // Zabezpieczenie: Odświeżamy UI od razu po podłączeniu klienta (dla dołączających w trakcie gry)
        if (ScoreboardUI.Instance != null)
        {
            ScoreboardUI.Instance.UpdateScoreboard(playerScores);
        }
    }

    [ServerCallback]
    private void RegisterMissingPlayers()
    {
        // Szukamy wszystkich graczy na mapie
        LobbyPlayer[] players = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            // Upewniamy się, że gracz zdążył załadować swój nick z profilu
            if (!string.IsNullOrEmpty(p.playerName) && p.playerName != "Gracz" && p.playerName != "Nieznajomy")
            {
                // Jeśli gracza nie ma jeszcze w tabeli wyników - wpisujemy mu bazowe 0 punktów
                if (!playerScores.ContainsKey(p.playerName))
                {
                    playerScores[p.playerName] = 0;
                }
            }
        }
    }

    // Ta funkcja odpala się u KAŻDEGO gracza, gdy słownik się zmieni (również gdy serwer doda "0")
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

        if (!playerScores.ContainsKey(playerName))
        {
            playerScores[playerName] = 0;
        }

        playerScores[playerName] += pointsChange;

        if (playerScores[playerName] < 0)
        {
            playerScores[playerName] = 0;
        }
    }
}