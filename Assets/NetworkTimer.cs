using UnityEngine;
using Mirror;
using TMPro;

public class NetworkTimer : NetworkBehaviour
{
    public float matchDuration = 120f;
    public TextMeshProUGUI timerText;

    [SyncVar] private double matchEndTime;
    [SyncVar] private bool isTimerRunning = false;

    public override void OnStartServer()
    {
        matchEndTime = NetworkTime.time + matchDuration;
        isTimerRunning = true;
    }

    void Update()
    {
        if (!isTimerRunning) return;

        float timeLeft = (float)(matchEndTime - NetworkTime.time);

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            if (isServer)
            {
                isTimerRunning = false;
                RpcEndGame();
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

    [ClientRpc]
    void RpcEndGame()
    {
        if (timerText != null) timerText.text = "KONIEC CZASU!";
        Debug.Log("Mecz dobiegł końca!");
    }
}