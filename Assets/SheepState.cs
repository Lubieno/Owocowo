using UnityEngine;
using Mirror;

public class SheepState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color currentColor = Color.white;

    [SyncVar] public string currentOwner = "";
    [SyncVar] public int currentSheepValue = 1; // Ile punktów jest warta ta owca (1 lub 2)

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
    public void ChangeColor(Color newColor, string newOwnerName, bool isDoublePoints)
    {
        if (currentOwner == newOwnerName) return;

        // 1. Odejmij punkty poprzedniemu właścicielowi (tyle ile owca była warta)
        if (!string.IsNullOrEmpty(currentOwner))
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.ChangeScore(currentOwner, -currentSheepValue);
        }

        // 2. Oblicz nową wartość punktową (zależy od boostera rzucającego)
        int pointsToGive = isDoublePoints ? 2 : 1;

        // 3. Dodaj punkty nowemu właścicielowi
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ChangeScore(newOwnerName, pointsToGive);

        // 4. Zaktualizuj stan owcy
        currentOwner = newOwnerName;
        currentSheepValue = pointsToGive; // Owca "zapamiętuje" swoją nową wartość
        currentColor = newColor;
    }
}