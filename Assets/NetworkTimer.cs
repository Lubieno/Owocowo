using UnityEngine;
using Mirror;
using TMPro;

public class NetworkTimer : NetworkBehaviour
{
    [Header("Ustawienia Czasu")]
    public float matchDuration = 120f; // 2 minuty w sekundach
    public TextMeshProUGUI timerText;

    // SyncVar gwarantuje, że nowo podłączeni klienci od razu dostaną ten czas
    [SyncVar] private double matchEndTime;
    [SyncVar] private bool isTimerRunning = false;

    public override void OnStartServer()
    {
        // NetworkTime.time to zsynchronizowany, uniwersalny czas dla wszystkich!
        matchEndTime = NetworkTime.time + matchDuration;
        isTimerRunning = true;
    }

    void Update()
    {
        if (!isTimerRunning) return;

        // Obliczamy ile zostało czasu do końca
        float timeLeft = (float)(matchEndTime - NetworkTime.time);

        if (timeLeft <= 0)
        {
            timeLeft = 0;

            // Tylko serwer może oficjalnie ogłosić koniec meczu
            if (isServer)
            {
                isTimerRunning = false;
                RpcEndGame();
            }
        }

        // Każdy gracz (i serwer, i klient) odświeża u siebie tekst na ekranie
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

    [ClientRpc]
    void RpcEndGame()
    {
        if (timerText != null)
        {
            timerText.text = "KONIEC CZASU!";
        }
        Debug.Log("Mecz dobiegł końca! Tu w przyszłości odpalimy podsumowanie.");
    }
}