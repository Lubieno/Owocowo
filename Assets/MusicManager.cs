using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Utwory")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Ustawienia")]
    [Tooltip("Czas płynnego przejścia w sekundach (skrócony dla szybszego efektu)")]
    public float fadeDuration = 0.5f;
    [Tooltip("Dokładna nazwa sceny, w której toczy się gra")]
    public string gameSceneName = "SampleScene";

    private AudioSource audioSource;
    private float masterVolume = 0.2f; // Domyślnie na start 20% (bardzo cicho)

    // Właściwość do kontrolowania głośności z poziomu suwaków
    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("MusicVolume", masterVolume); // Zapisujemy ustawienie w pamięci gry

            // Jeśli akurat nie trwa zmiana utworu, od razu zmieniamy głośność głośnika
            if (!isFading)
            {
                audioSource.volume = masterVolume;
            }
        }
    }

    private bool isFading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            // Wczytaj zapisaną głośność, a jeśli gracz odpala grę 1 raz – daj domyślne 0.2f
            masterVolume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
            audioSource.volume = masterVolume;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip targetClip = (scene.name == gameSceneName) ? gameMusic : menuMusic;

        if (audioSource.clip != targetClip)
        {
            StartCoroutine(CrossfadeMusic(targetClip));
        }
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        isFading = true;
        float startVolume = audioSource.volume;

        // Płynne wyciszanie starego utworu (Fade Out)
        if (audioSource.isPlaying)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 0;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        // Płynne podgłaśnianie nowego utworu do wybranego poziomu MasterVolume (Fade In)
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, masterVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = masterVolume;
        isFading = false;
    }
}