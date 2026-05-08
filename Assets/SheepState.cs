using UnityEngine;
using Mirror;

public class SheepState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color currentColor = Color.white;

    private Renderer[] allRenderers;

    void Awake()
    {
        // Szukamy wszystkich rendererów (modelu z Blendera) pod owcą
        allRenderers = GetComponentsInChildren<Renderer>();
    }

    void OnColorChanged(Color oldColor, Color newColor)
    {
        // Jeśli Renderery nie zostały znalezione w Awake, szukamy ich teraz
        if (allRenderers == null || allRenderers.Length == 0)
            allRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in allRenderers)
        {
            if (r != null)
            {
                // --- KLUCZOWA ZMIANA: FILTROWANIE GŁOWY ---
                // Sprawdzamy, czy nazwa obiektu zawiera słowo "Head" lub "Glowa".
                // Jeśli Twój obiekt w Unity nazywa się inaczej (np. "SheepHead"), wpisz to tutaj!
                if (r.gameObject.name.Contains("head"))
                {
                    continue; // To słowo sprawia, że skrypt pomija ten element i nie zmienia jego koloru
                }

                // Jeśli to nie jest głowa, normalnie zmieniamy kolor
                r.material.color = newColor;
            }
        }
    }

    [Server]
    public void ChangeColor(Color newColor)
    {
        Debug.Log($"[SERWER] Zmieniam kolor owcy na: {newColor}");
        currentColor = newColor;
    }
}
