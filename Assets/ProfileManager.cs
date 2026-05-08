using UnityEngine;
using System.Collections.Generic;
using System.IO;

// Klasa przechowująca dane pojedynczego profilu
[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    // Tutaj w przyszłości możemy dodać np. level, statystyki, wybrany kolor itp.
}

// Opakowanie dla listy profili (potrzebne, bo Unity JsonUtility nie radzi sobie z samymi listami)
[System.Serializable]
public class ProfileDataWrapper
{
    public List<PlayerProfile> profiles = new List<PlayerProfile>();
}

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance; // Singleton - dostęp z każdego miejsca w kodzie

    public ProfileDataWrapper dataWrapper = new ProfileDataWrapper();
    public PlayerProfile currentProfile; // Aktualnie wybrany profil

    private string saveFilePath;

    void Awake()
    {
        // Gwarantujemy, że istnieje tylko JEDEN ProfileManager w całej grze
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ten obiekt nie zniknie przy zmianie sceny!

            // Ścieżka zapisu (w Windowsie to np. AppData/LocalLow/NazwaFirmy/NazwaGry)
            saveFilePath = Application.persistentDataPath + "/profiles.json";
            LoadProfiles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveProfiles()
    {
        string json = JsonUtility.ToJson(dataWrapper, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Zapisano profile do: " + saveFilePath);
    }

    public void LoadProfiles()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            dataWrapper = JsonUtility.FromJson<ProfileDataWrapper>(json);
        }
        else
        {
            dataWrapper = new ProfileDataWrapper(); // Brak pliku = pusta lista
        }
    }

    public void CreateProfile(string newName)
    {
        PlayerProfile newProfile = new PlayerProfile { playerName = newName };
        dataWrapper.profiles.Add(newProfile);
        SaveProfiles();
    }

    public void DeleteProfile(int index)
    {
        if (index >= 0 && index < dataWrapper.profiles.Count)
        {
            dataWrapper.profiles.RemoveAt(index);
            SaveProfiles();
        }
    }
}