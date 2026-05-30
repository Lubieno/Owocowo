using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;
}

[System.Serializable]
public class ProfileDataWrapper
{
    public List<PlayerProfile> profiles = new List<PlayerProfile>();
}

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;

    public ProfileDataWrapper dataWrapper = new ProfileDataWrapper();
    public PlayerProfile currentProfile;

    // --- NOWOŚĆ: Zmienna-plecak przechowująca kolor przy zmianie sceny! ---
    public Color currentSessionColor = Color.blue;

    private string saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            dataWrapper = new ProfileDataWrapper();
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