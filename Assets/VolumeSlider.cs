using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        // Zabezpieczenie wartości suwaka od 0 do 1
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // Jeśli MusicManager już istnieje, ustaw suwak w pozycji zapisanej głośności
        if (MusicManager.Instance != null)
        {
            slider.value = MusicManager.Instance.MasterVolume;
        }

        // Nakazujemy suwakowi nasłuchiwać zmian (gdy gracz nim poruszy)
        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float newValue)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.MasterVolume = newValue;
        }
    }
}