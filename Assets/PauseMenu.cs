using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    [Header("UI Elements")]
    public GameObject pauseMenuUI;

    void Update()
    {
        // Sprawdzamy wciśnięcie Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;

        // Przy wracaniu do gry blokujemy kursor z powrotem
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;

        // Pokazujemy kursor, żeby dało się klikać w przyciski
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Funkcja dla przycisku Wyjdź
    public void QuitToMenu()
    {
        Debug.Log("Wychodzenie do menu...");

        // Bardzo ważne: Musimy zresetować stan kursora przed wyjściem!
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = false;

        // Zatrzymujemy Mirrora w zależności od tego, czy jesteśmy Hostem czy Klientem
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else
        {
            // Jeśli coś pójdzie nie tak z Mirror, ładujemy scenę ręcznie
            SceneManager.LoadScene("MainMenu");
        }
    }
}