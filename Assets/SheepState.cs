using UnityEngine;
using Mirror;

public class SheepState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color currentColor = Color.white;

    [SyncVar] public string currentOwner = "";
    [SyncVar] public int currentSheepValue = 1;

    [Header("Dźwięki")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    private Renderer[] allRenderers;

    void Awake()
    {
        allRenderers = GetComponentsInChildren<Renderer>();
    }

    public override void OnStartClient()
    {
        // Kiedy gracz dołącza w trakcie gry, musi widzieć prawidłowe kolory owiec
        ApplyColorToSheep(currentColor);
    }

    void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColorToSheep(newColor);

        if (audioSource != null && hitSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hitSound);
        }
    }

    // Nowa, oddzielna funkcja bezpiecznie przypisująca kolor
    void ApplyColorToSheep(Color colorToSet)
    {
        if (allRenderers == null || allRenderers.Length == 0)
            allRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in allRenderers)
        {
            if (r != null)
            {
                if (r.gameObject.name.Contains("head")) continue;
                r.material.color = colorToSet;
            }
        }
    }

    [Server]
    public void ChangeColor(Color newColor, string newOwnerName, bool isDoublePoints)
    {
        if (currentOwner == newOwnerName) return;

        if (!string.IsNullOrEmpty(currentOwner) && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ChangeScore(currentOwner, -currentSheepValue);
        }

        int pointsToGive = isDoublePoints ? 2 : 1;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ChangeScore(newOwnerName, pointsToGive);
        }

        currentOwner = newOwnerName;
        currentSheepValue = pointsToGive;

        // Zmiana SyncVar - to automatycznie odpali 'OnColorChanged' u każdego
        currentColor = newColor;
    }
}