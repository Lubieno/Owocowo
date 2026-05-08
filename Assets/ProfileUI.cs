using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ProfileUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown profileDropdown;
    public TMP_InputField nameInput;
    public Button createButton;
    public Button deleteButton;
    public Button playButton;

    void Start()
    {
        RefreshDropdown();
    }

    // Odświeża rozwijaną listę na podstawie danych z ProfileManagera
    public void RefreshDropdown()
    {
        profileDropdown.ClearOptions();
        List<string> options = new List<string>();

        var profiles = ProfileManager.Instance.dataWrapper.profiles;

        if (profiles.Count == 0)
        {
            options.Add("Brak profili...");
            deleteButton.interactable = false;
            playButton.interactable = false;
        }
        else
        {
            foreach (var profile in profiles)
            {
                options.Add(profile.playerName);
            }
            deleteButton.interactable = true;
            playButton.interactable = true;
        }

        profileDropdown.AddOptions(options);
    }

    // Podpinamy to pod przycisk "Stwórz"
    public void OnCreateButtonClicked()
    {
        string newName = nameInput.text.Trim();
        if (!string.IsNullOrEmpty(newName))
        {
            ProfileManager.Instance.CreateProfile(newName);
            nameInput.text = ""; // Czyścimy pole
            RefreshDropdown();

            // Wybieramy nowo dodany profil (ostatni na liście)
            profileDropdown.value = ProfileManager.Instance.dataWrapper.profiles.Count - 1;
        }
    }

    // Podpinamy to pod przycisk "Usuń"
    public void OnDeleteButtonClicked()
    {
        int selectedIndex = profileDropdown.value;
        if (ProfileManager.Instance.dataWrapper.profiles.Count > 0)
        {
            ProfileManager.Instance.DeleteProfile(selectedIndex);
            RefreshDropdown();
        }
    }

    // Podpinamy to pod przycisk "Graj"
    public void OnPlayButtonClicked()
    {
        int selectedIndex = profileDropdown.value;
        if (ProfileManager.Instance.dataWrapper.profiles.Count > 0)
        {
            // Zapisujemy w Singletonie, który profil wybraliśmy, by przekazać go do kolejnych scen
            ProfileManager.Instance.currentProfile = ProfileManager.Instance.dataWrapper.profiles[selectedIndex];

            Debug.Log("Zalogowano jako: " + ProfileManager.Instance.currentProfile.playerName);

            // Ładujemy Main Menu (Zmień "MainMenu" na nazwę swojej sceny z menu głównym, jeśli jest inna!)
            SceneManager.LoadScene("MainMenu");
        }
    }
}