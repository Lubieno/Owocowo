using UnityEngine;
using Mirror;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))] public string playerName = "Gracz";
    [SyncVar(hook = nameof(OnColorChanged))] public Color playerColor = Color.blue;
    [SyncVar(hook = nameof(OnReadyChanged))] public bool isReady = false;

    // Kopia listy kolorów, żeby gracz wiedział, z czego wybierać ratunkowo
    private Color[] fallbackColors = new Color[] { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };

    public override void OnStartLocalPlayer()
    {
        string myName = "Nieznajomy";
        Color myColor = Color.blue;

        if (ProfileManager.Instance != null)
        {
            if (ProfileManager.Instance.currentProfile != null)
                myName = ProfileManager.Instance.currentProfile.playerName;

            myColor = ProfileManager.Instance.currentSessionColor;
        }

        // --- ZMIANA: Zanim ustawimy kolor, sprawdzamy czy nie wchodzimy komuś w paradę ---
        myColor = GetFirstFreeColor(myColor);

        // Zapisujemy nowy, poprawny kolor do plecaka
        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.currentSessionColor = myColor;
        }

        CmdSetName(myName);
        CmdSetColor(myColor);
    }

    // --- NOWOŚĆ: System szukania wolnego koloru przy starcie ---
    private Color GetFirstFreeColor(Color preferredColor)
    {
        LobbyPlayer[] players = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);

        // Krok 1: Sprawdźmy, czy nasz ulubiony kolor jest wolny
        bool preferredTaken = false;
        foreach (var p in players)
        {
            if (p != this && p.playerColor == preferredColor)
            {
                preferredTaken = true;
                break;
            }
        }

        if (!preferredTaken) return preferredColor; // Super, nikt go nie ma!

        // Krok 2: Jeśli ulubiony jest zajęty, bierzemy pierwszy lepszy wolny
        foreach (Color c in fallbackColors)
        {
            bool taken = false;
            foreach (var p in players)
            {
                if (p != this && p.playerColor == c)
                {
                    taken = true;
                    break;
                }
            }
            if (!taken) return c; // Oddajemy wolny kolor
        }

        // W ostateczności (choć mamy 5 kolorów a maksymalnie 4 graczy, więc to niemożliwe)
        return preferredColor;
    }

    [Command] public void CmdSetName(string name) { playerName = name; }
    [Command] public void CmdSetColor(Color color) { playerColor = color; }
    [Command] public void CmdSetReady(bool ready) { isReady = ready; }

    void OnNameChanged(string oldName, string newName) { LobbyUIManager.Instance?.UpdateUI(); }
    void OnColorChanged(Color oldColor, Color newColor) { LobbyUIManager.Instance?.UpdateUI(); }
    void OnReadyChanged(bool oldReady, bool newReady) { LobbyUIManager.Instance?.UpdateUI(); }

    public override void OnStartClient() { LobbyUIManager.Instance?.UpdateUI(); }
    public override void OnStopClient() { LobbyUIManager.Instance?.UpdateUI(); }
}