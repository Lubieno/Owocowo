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
    public AudioClip hitSound; // Dźwięk, gdy owca obrywa owocem

    private Renderer[] allRenderers;

    void Awake()
    {
        allRenderers = GetComponentsInChildren<Renderer>();
    }

    void OnColorChanged(Color oldColor, Color newColor)
    {
        if (allRenderers == null || allRenderers.Length == 0)
            allRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in allRenderers)
        {
            if (r != null)
            {
                if (r.gameObject.name.Contains("head")) continue;
                r.material.color = newColor;
            }
        }

        // --- NOWE: Odtwarzamy dźwięk trafienia u KAŻDEGO gracza ---
        // Hook wykonuje się automatycznie u wszystkich, więc to idealne miejsce!
        if (audioSource != null && hitSound != null)
        {
            // Zmieniamy lekko wysokość dźwięku (pitch), żeby każde trafienie brzmiało trochę inaczej
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hitSound);
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
        currentColor = newColor;
    }
}