using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance;

    [Header("Sloty Graczy")]
    public GameObject[] playerSlots;
    public TextMeshProUGUI[] nameTexts;
    public Button[] colorButtons;
    public Toggle[] readyToggles;

    [Header("Globalne UI")]
    public TextMeshProUGUI readyCountText;
    public Button startButton;

    // Pula kolorów do wyboru
    private Color[] availableColors = new Color[] { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };
    private int myColorIndex = 0;

    void Awake() { Instance = this; }

    void Start()
    {
        if (startButton != null) startButton.gameObject.SetActive(false);
        UpdateUI();
    }

    public void UpdateUI()
    {
        LobbyPlayer[] players = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);
        int readyCount = 0;

        for (int i = 0; i < 4; i++)
        {
            if (i < players.Length)
            {
                if (playerSlots != null && playerSlots.Length > i && playerSlots[i] != null) playerSlots[i].SetActive(true);
                if (nameTexts != null && nameTexts.Length > i && nameTexts[i] != null) nameTexts[i].text = players[i].playerName;

                if (colorButtons != null && colorButtons.Length > i && colorButtons[i] != null)
                {
                    Image img = colorButtons[i].GetComponent<Image>();
                    if (img != null) img.color = players[i].playerColor;
                    colorButtons[i].interactable = players[i].isLocalPlayer;
                }

                if (readyToggles != null && readyToggles.Length > i && readyToggles[i] != null)
                {
                    readyToggles[i].SetIsOnWithoutNotify(players[i].isReady);
                    readyToggles[i].interactable = players[i].isLocalPlayer;
                }

                if (players[i].isReady) readyCount++;
            }
            else
            {
                if (playerSlots != null && playerSlots.Length > i && playerSlots[i] != null) playerSlots[i].SetActive(false);
            }
        }

        if (readyCountText != null) readyCountText.text = $"{readyCount} / {players.Length}";

        if (NetworkServer.active && startButton != null)
        {
            bool allReady = (readyCount == players.Length && players.Length > 0);
            startButton.gameObject.SetActive(true);
            startButton.interactable = allReady;
        }
        else if (startButton != null) startButton.gameObject.SetActive(false);
    }

    public void OnMyReadyToggled(bool isReady)
    {
        if (NetworkClient.localPlayer == null) return;
        LobbyPlayer localPlayer = NetworkClient.localPlayer.GetComponent<LobbyPlayer>();
        if (localPlayer != null) localPlayer.CmdSetReady(isReady);
    }

    // --- ZMIANA: Mądre klikanie koloru ---
    public void OnMyColorClicked()
    {
        if (NetworkClient.localPlayer == null) return;
        LobbyPlayer localPlayer = NetworkClient.localPlayer.GetComponent<LobbyPlayer>();

        if (localPlayer != null)
        {
            // Szukamy najbliższego WOLNEGO koloru (maksymalnie tyle prób, ile jest kolorów)
            for (int i = 0; i < availableColors.Length; i++)
            {
                myColorIndex = (myColorIndex + 1) % availableColors.Length;
                Color nextColor = availableColors[myColorIndex];

                if (!IsColorTaken(nextColor))
                {
                    // Znaleźliśmy wolny! Zapisujemy i przerywamy pętlę.
                    if (ProfileManager.Instance != null)
                    {
                        ProfileManager.Instance.currentSessionColor = nextColor;
                    }
                    localPlayer.CmdSetColor(nextColor);
                    break;
                }
            }
        }
    }

    // --- NOWOŚĆ: Funkcja sprawdzająca czy kolor jest wolny ---
    private bool IsColorTaken(Color colorToCheck)
    {
        LobbyPlayer[] players = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);
        foreach (LobbyPlayer p in players)
        {
            // Jeśli inny gracz (nie my) ma już ten kolor, zgłaszamy że jest zajęty
            if (p.playerColor == colorToCheck && !p.isLocalPlayer)
            {
                return true;
            }
        }
        return false;
    }

    public void StartGame()
    {
        if (NetworkServer.active) NetworkManager.singleton.ServerChangeScene("SampleScene");
    }
}