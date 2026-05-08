using UnityEngine;
using Mirror;

public enum BoosterType { Speed, DoubleJump, SuperJump }

public class Booster : NetworkBehaviour
{
    // SyncVar informuje wszystkich klientów, jakiego typu jest ten booster
    [SyncVar(hook = nameof(OnTypeChanged))]
    public BoosterType type;

    public float duration = 10f;

    [Header("Wygląd Boosterów (Przypisz w Inspektorze)")]
    public Material speedMaterial;       // np. Czerwony
    public Material doubleJumpMaterial;  // np. Niebieski
    public Material superJumpMaterial;   // np. Zielony

    private Renderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponentInChildren<Renderer>();
    }

    public override void OnStartServer()
    {
        // Serwer losuje typ boostera w momencie jego pojawienia się na mapie
        type = (BoosterType)Random.Range(0, 3);
    }

    // Ta funkcja odpala się u KAŻDEGO gracza, zmieniając kolor modelu
    void OnTypeChanged(BoosterType oldType, BoosterType newType)
    {
        if (meshRenderer == null) return;

        switch (newType)
        {
            case BoosterType.Speed: meshRenderer.material = speedMaterial; break;
            case BoosterType.DoubleJump: meshRenderer.material = doubleJumpMaterial; break;
            case BoosterType.SuperJump: meshRenderer.material = superJumpMaterial; break;
        }
    }

    [ServerCallback] // Tylko serwer przetwarza podniesienie
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEffects effects = other.GetComponent<PlayerEffects>();
            if (effects != null)
            {
                effects.ApplyBooster(type, duration);
                NetworkServer.Destroy(gameObject); // Usuwamy z mapy po podniesieniu
            }
        }
    }
}
