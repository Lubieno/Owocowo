using UnityEngine;
using Mirror;

public enum BoosterType { Speed, ScoreMultiplier }

public class Booster : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTypeChanged))]
    public BoosterType type;

    public float duration = 10f;

    [Header("Materiały Wyglądu")]
    public Material speedMaterial;    // Przypisz np. czerwony
    public Material scoreMaterial;   // Przypisz np. żółty

    private Renderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponentInChildren<Renderer>();
    }

    public override void OnStartServer()
    {
        // Losujemy typ przy spawnie
        type = (BoosterType)Random.Range(0, 2);
    }

    void OnTypeChanged(BoosterType oldType, BoosterType newType)
    {
        if (meshRenderer == null) return;

        if (newType == BoosterType.Speed)
            meshRenderer.material = speedMaterial;
        else
            meshRenderer.material = scoreMaterial;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEffects effects = other.GetComponent<PlayerEffects>();
            if (effects != null)
            {
                effects.ApplyBooster(type, duration);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}