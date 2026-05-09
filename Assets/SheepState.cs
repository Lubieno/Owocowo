using UnityEngine;
using Mirror;

public class SheepState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color currentColor = Color.white;

    // --- ZMIANA: Owca pamięta obecnego właściciela ---
    [SyncVar] public string currentOwner = "";

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
    }

    [Server]
    public void ChangeColor(Color newColor, string newOwnerName)
    {
        // Jeśli ten sam gracz ponownie trafił swoją owcę, nic nie robimy
        if (currentOwner == newOwnerName) return;

        // Jeśli owca miała wcześniej innego właściciela, ODBIERZ MU PUNKT
        if (!string.IsNullOrEmpty(currentOwner))
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.ChangeScore(currentOwner, -1);
        }

        // Dodaj punkt NOWEMU właścicielowi
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ChangeScore(newOwnerName, 1);

        // Zaktualizuj stan owcy
        currentOwner = newOwnerName;
        currentColor = newColor;
    }
}