using UnityEngine;
using Mirror; // Wyrzucamy SceneManagement, wrzucamy Mirrora

public class MainMenu : MonoBehaviour
{
    // Ten przycisk zastępuje stare "PlayGame"
    public void PlayAsHost()
    {
        Debug.Log("Uruchamiam serwer i ładuję scenę sieciową...");
        
        // Ta jedna linijka robi dwie rzeczy naraz: odpala serwer i automatycznie 
        // ładuje "SampleScene" (bo ustawiłeś ją w polu Online Scene w Network Managerze)
        NetworkManager.singleton.StartHost(); 
    }

    // Nowy przycisk dla kolegów
    public void PlayAsClient()
    {
        Debug.Log("Łączę się z serwerem...");
        NetworkManager.singleton.networkAddress = "localhost"; // Na razie testujemy na jednym PC
        NetworkManager.singleton.StartClient();
    }

    // Twoja stara funkcja wyjścia - zostaje całkowicie bez zmian!
    public void QuitGame()
    {
        Debug.Log("Gra została wyłączona!");
        Application.Quit();
    }
}