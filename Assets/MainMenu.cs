using UnityEngine;
using UnityEngine.SceneManagement; // Wymagane do przełączania scen

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Ładuje scenę z grą. Nazwa musi dokładnie pasować do Waszej sceny z rozgrywką!
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        // Wypisze tekst w konsoli, żebyśmy widzieli, że przycisk działa w edytorze
        Debug.Log("Gra została wyłączona!");

        // Zamyka grę (zadziała dopiero po zbudowaniu pliku .exe)
        Application.Quit();
    }
}