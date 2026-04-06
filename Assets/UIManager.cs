using UnityEngine;
using UnityEngine.SceneManagement; // Do ładowania scen
using TMPro; // Do obsługi nowoczesnego tekstu w Unity

public class UIManager : MonoBehaviour
{
    [Header("HUD Rozgrywki")]
    public TextMeshProUGUI timerText; // Tekst licznika czasu
    public float gameTime = 120f; // 2 minuty na rundę

    private bool isGameActive = false;

    void Start()
    {
        // Jeśli ten skrypt jest na scenie z grą, uruchom czas
        if (timerText != null)
        {
            isGameActive = true;
        }
    }

    void Update()
    {
        if (isGameActive && gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (gameTime <= 0)
            {
                EndGame();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        // Formatowanie czasu na minuty i sekundy (np. 1:30)
        int minutes = Mathf.FloorToInt(gameTime / 60F);
        int seconds = Mathf.FloorToInt(gameTime - minutes * 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private void EndGame()
    {
        isGameActive = false;
        timerText.text = "KONIEC CZASU!";
        // TODO: Wyświetlenie ekranu końcowego i podsumowania
    }

    // --- FUNKCJE DLA PRZYCISKÓW W MENU GŁÓWNYM --- //

    public void StartGame()
    {
        // Ładuje scenę z grą. Pamiętajcie, by dodać sceny w File -> Build Settings!
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Wychodzenie z gry...");
        Application.Quit();
    }
}
