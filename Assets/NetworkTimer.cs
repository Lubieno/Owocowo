using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;
using System.Linq;

public class NetworkTimer : NetworkBehaviour
{
    public float matchDuration = 120f;
    public TextMeshProUGUI timerText;

    [Header("UI Końcowe (Osobny Canvas)")]
    public GameObject postGamePanel;
    public TextMeshProUGUI postGameScoreText;
    public Camera postGameCamera;

    [Header("Przyciski Akcji (Pojawiają się po 5s)")]
    public GameObject actionButtonsPanel;
    public TextMeshProUGUI playAgainText;
    public TextMeshProUGUI votingStatusText; // Wyświetla np. "Gotowi: 1/4"

    [SyncVar] private double matchEndTime;
    [SyncVar] private bool isTimerRunning = false;

    // --- Zmienne do systemu głosowania ---
    [SyncVar] private bool isPostGame = false;
    [SyncVar(hook = nameof(OnVotingStatusUpdated))] private int readyCount = 0;
    [SyncVar(hook = nameof(OnVotingStatusUpdated))] private int totalPlayers = 0;
    private bool hasVoted = false;

    public override void OnStartServer()
    {
        matchEndTime = NetworkTime.time + matchDuration;
        isTimerRunning = true;
    }

    void Update()
    {
        // --- LOGIKA GŁOSOWANIA (Tylko Serwer) ---
        if (isServer && isPostGame)
        {
            totalPlayers = NetworkServer.connections.Count;

            if (readyCount >= totalPlayers && totalPlayers > 0)
            {
                NetworkManager.singleton.ServerChangeScene("LobbyScene");
                isPostGame = false;
            }
        }

        // --- LOGIKA TIMERA ---
        if (!isTimerRunning) return;

        float timeLeft = (float)(matchEndTime - NetworkTime.time);

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            if (isServer)
            {
                isTimerRunning = false;
                ServerEndMatch();
            }
        }
        UpdateTimerDisplay(timeLeft);
    }

    private void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    [Server]
    private void ServerEndMatch()
    {
        isPostGame = true;

        SheepState[] allSheep = FindObjectsByType<SheepState>(FindObjectsSortMode.None);
        foreach (SheepState sheep in allSheep) NetworkServer.Destroy(sheep.gameObject);

        RpcStartEndGameSequence();
    }

    [ClientRpc]
    void RpcStartEndGameSequence()
    {
        if (timerText != null) timerText.text = "KONIEC CZASU!";
        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(2.5f);

        if (NetworkClient.localPlayer != null)
        {
            NetworkClient.localPlayer.GetComponent<PlayerMovement>().enabled = false;
            NetworkClient.localPlayer.GetComponent<PlayerThrow>().enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Camera.main != null && postGameCamera != null)
        {
            Camera.main.transform.SetParent(null);
            yield return StartCoroutine(SmoothCameraTransition(Camera.main.transform, postGameCamera.transform, 2.0f));
        }

        if (postGamePanel != null) postGamePanel.SetActive(true);
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);

        GeneratePostGameScoreboard();

        yield return new WaitForSeconds(5.0f);

        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(true);
        OnVotingStatusUpdated(0, 0);

        if (!isServer && playAgainText != null)
        {
            playAgainText.text = "Zostań w grze\n<size=50%>(Oczekuj na Hosta)</size>";
        }
    }

    private IEnumerator SmoothCameraTransition(Transform movingCamera, Transform targetCamera, float duration)
    {
        Vector3 startPosition = movingCamera.position;
        Quaternion startRotation = movingCamera.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            movingCamera.position = Vector3.Lerp(startPosition, targetCamera.position, elapsedTime / duration);
            movingCamera.rotation = Quaternion.Lerp(startRotation, targetCamera.rotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        movingCamera.position = targetCamera.position;
        movingCamera.rotation = targetCamera.rotation;
    }

    private void GeneratePostGameScoreboard()
    {
        if (postGameScoreText == null || ScoreManager.Instance == null) return;

        // 1. POBRANIE KOLORÓW GRACZY
        LobbyPlayer[] players = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);
        System.Collections.Generic.Dictionary<string, string> colorMap = new System.Collections.Generic.Dictionary<string, string>();

        foreach (var p in players)
        {
            colorMap[p.playerName] = "#" + ColorUtility.ToHtmlStringRGB(p.playerColor);
        }

        // 2. SORTOWANIE WYNIKÓW
        var sortedScores = ScoreManager.Instance.playerScores
            .OrderByDescending(player => player.Value)
            .ToList();

        // 3. BUDOWANIE TABELI 
        string finalScoreText = "<color=#A0A0A0>POZYCJA <pos=25%>GRACZ <pos=65%>WYNIK <pos=85%>KOLOR</color>\n\n";

        int place = 1;
        foreach (var score in sortedScores)
        {
            string pName = score.Key;

            string hexColor = colorMap.ContainsKey(pName) ? colorMap[pName] : "#FFFFFF";

            string placeColor = "#FFFFFF";
            if (place == 1) placeColor = "#FFD700"; // Złoto
            else if (place == 2) placeColor = "#C0C0C0"; // Srebro
            else if (place == 3) placeColor = "#CD7F32"; // Brąz

            finalScoreText += $"<color={placeColor}>{place}.</color> <pos=25%>{pName} <pos=65%><color={placeColor}>{score.Value} PKT</color> <pos=85%><color={hexColor}>■</color>\n\n";

            place++;
        }

        if (sortedScores.Count == 0)
        {
            finalScoreText += "<pos=35%><i>Brak wyników!</i>";
        }

        postGameScoreText.text = finalScoreText;
    }

    // --- SYSTEM GŁOSOWANIA INTERFEJS ---

    private void OnVotingStatusUpdated(int oldVal, int newVal)
    {
        if (votingStatusText != null)
        {
            votingStatusText.text = $"Gotowi na rewanż: <color=yellow>{readyCount} / {totalPlayers}</color>";
        }
    }

    public void OnPlayAgainClicked()
    {
        if (hasVoted) return;

        hasVoted = true;

        if (playAgainText != null) playAgainText.text = "<color=green>Zatwierdzono!</color>";

        CmdPlayerReady();
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayerReady()
    {
        readyCount++;
    }

    public void OnQuitClicked()
    {
        if (isServer) NetworkManager.singleton.StopHost();
        else NetworkManager.singleton.StopClient();
    }
}