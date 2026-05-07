using UnityEngine;
using Mirror; // Obsługa sieci

public class SheepState : NetworkBehaviour
{
    // [SyncVar] oznacza: "Jeśli serwer zmieni tę zmienną, zaktualizuj ją u wszystkich klientów".
    // hook = nameof(OnColorChanged) oznacza: "Kiedy dostaniesz nowy kolor, odpal tę funkcję, żeby pomalować model".
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color currentColor = Color.white;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Ta funkcja odpala się u KAŻDEGO gracza w momencie zmiany koloru
    void OnColorChanged(Color oldColor, Color newColor)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = newColor;
        }
    }

    // [Server] oznacza, że TYLKO serwer może wywołać tę funkcję
    [Server]
    public void ChangeColor(Color newColor)
    {
        currentColor = newColor; // Zmiana tej zmiennej automatycznie odpali 'OnColorChanged' w sieci
    }
}