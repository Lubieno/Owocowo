using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Usunęliśmy wszystko związane z czasem – teraz rządzi tym NetworkTimer.
    // Zostawiamy jednak funkcje do Menu, gdybyście wciąż z nich korzystali w innych scenach!

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Wychodzenie z gry...");
        Application.Quit();
    }
}