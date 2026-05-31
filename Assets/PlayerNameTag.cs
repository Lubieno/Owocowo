using UnityEngine;
using Mirror;
using TMPro;

public class PlayerNameTag : NetworkBehaviour
{
    [Header("Ustawienia UI")]
    public GameObject nameTagCanvas;
    public TextMeshProUGUI nameText;

    private Camera mainCamera;
    private LobbyPlayer lobbyPlayer;

    // ZMIANA: Zamiast 'isInitialized' będziemy pamiętać ostatni stan nicku i koloru
    private string lastKnownName = "";
    private Color lastKnownColor = Color.clear;

    void Start()
    {
        lobbyPlayer = GetComponent<LobbyPlayer>();
        mainCamera = Camera.main;

        // Ukrywamy własną tabliczkę dla lokalnego gracza (żeby nie zasłaniała widoku FPS)
        if (isLocalPlayer && nameTagCanvas != null)
        {
            nameTagCanvas.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (isLocalPlayer) return;

        if (mainCamera == null) mainCamera = Camera.main;

        // Obracanie w stronę kamery (jak w Minecraft)
        if (mainCamera != null && nameTagCanvas != null)
        {
            nameTagCanvas.transform.rotation = mainCamera.transform.rotation;
        }

        // ZMIANA: Jeżeli nick lub kolor z serwera różnią się od tego na tabliczce -> zaktualizuj!
        if (lobbyPlayer != null)
        {
            if (lobbyPlayer.playerName != lastKnownName || lobbyPlayer.playerColor != lastKnownColor)
            {
                UpdateNameTag();
            }
        }
    }

    public void UpdateNameTag()
    {
        if (nameText == null || lobbyPlayer == null) return;

        // Nakładamy nowe dane na UI
        nameText.text = lobbyPlayer.playerName;
        nameText.color = lobbyPlayer.playerColor;

        // Zapisujemy nowy stan do pamięci, żeby nie odświeżać tekstu bez sensu co klatkę
        lastKnownName = lobbyPlayer.playerName;
        lastKnownColor = lobbyPlayer.playerColor;
    }
}