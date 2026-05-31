using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI Notification Elements")]
    public GameObject notificationPanel; // Nasz prostokąt (Panel/Image), który będzie znikał
    public TextMeshProUGUI statusText;   // Tekst wewnątrz tego prostokąta

    private Coroutine notificationCoroutine;

    void Start()
    {
        // Kursor ma być zawsze odblokowany i widoczny w Menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Na starcie gry twardo ukrywamy prostokąt z komunikatem
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    public void PlayAsHost()
    {
        ShowNotification("Uruchamianie serwera...", 2.0f);
        NetworkManager.singleton.StartHost();
    }

    public void PlayAsClient()
    {
        ShowNotification("Łączenie z serwerem...", 2.5f);
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartClient();

        // Uruchamiamy sprawdzanie połączenia
        StartCoroutine(CheckConnectionRoutine());
    }

    private IEnumerator CheckConnectionRoutine()
    {
        // Czekamy 2.5 sekundy. Jeśli połączymy się z Lobby, scena się zmieni i skrypt się wyłączy.
        yield return new WaitForSeconds(2.5f);

        // Jeśli po 2.5 sekundy nadal tu jesteśmy i nie ma połączenia:
        if (!NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();

            // Wyświetlamy komunikat o błędzie w prostokącie na dokładnie 2.5 sekundy
            ShowNotification("<color=red>Nie można dołączyć!</color>\nLobby jest pełne lub serwer nie istnieje.", 2.5f);
        }
    }

    // --- NOWOŚĆ: Funkcja do wywoływania znikającego komunikatu ---
    public void ShowNotification(string message, float duration)
    {
        if (notificationPanel == null || statusText == null) return;

        // Jeśli poprzedni komunikat jeszcze odlicza czas, przerywamy go,
        // żeby czasy nowego i starego komunikatu się nie pokłóciły
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        // Odpalamy nowe odliczanie zniknięcia
        notificationCoroutine = StartCoroutine(NotificationRoutine(message, duration));
    }

    private IEnumerator NotificationRoutine(string message, float duration)
    {
        statusText.text = message;       // Ustawiamy tekst
        notificationPanel.SetActive(true); // Pokazujemy prostokąt

        yield return new WaitForSeconds(duration); // Czekamy zadany czas (np. 2.5s)

        notificationPanel.SetActive(false); // Ukrywamy prostokąt
        notificationCoroutine = null;
    }

    public void QuitGame()
    {
        Debug.Log("Gra została wyłączona!");
        Application.Quit();
    }
}