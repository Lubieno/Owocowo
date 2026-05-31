using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // NOWOŚĆ: Potrzebne do wykrywania myszki!

public class GlobalButtonSounds : MonoBehaviour
{
    [Header("Dźwięki")]
    public AudioClip clickSound;
    public AudioClip hoverSound; // NOWOŚĆ: Miejsce na dźwięk najechania

    void Start()
    {
        // Znajdujemy wszystkie przyciski
        Button[] allButtons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in allButtons)
        {
            // --- 1. DŹWIĘK KLIKNIĘCIA ---
            btn.onClick.AddListener(PlayClickSound);

            // --- 2. DŹWIĘK NAJECHANIA (HOVER) ---
            // Dodajemy komponent EventTrigger do przycisku (jeśli go nie ma)
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = btn.gameObject.AddComponent<EventTrigger>();
            }

            // Tworzymy nową akcję reagującą na wejście kursora (PointerEnter)
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { PlayHoverSound(); });

            trigger.triggers.Add(hoverEntry);
        }
    }

    void PlayClickSound()
    {
        PlaySound(clickSound);
    }

    void PlayHoverSound()
    {
        PlaySound(hoverSound);
    }

    // Wspólna funkcja tworząca głośnik, żeby nie powtarzać kodu
    void PlaySound(AudioClip clipToPlay)
    {
        if (clipToPlay != null)
        {
            GameObject soundObject = new GameObject("TemporaryUI_Sound");
            DontDestroyOnLoad(soundObject);

            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = clipToPlay;
            source.spatialBlend = 0f;

            source.Play();

            Destroy(soundObject, clipToPlay.length);
        }
    }
}