using UnityEngine;
using Mirror;

public class PlayerEffects : NetworkBehaviour
{
    private PlayerMovement movement;

    [SyncVar] public bool hasScoreMultiplier = false;

    // Liczniki czasu trwania boosterów
    private float speedTimer = 0f;
    private float scoreTimer = 0f;

    // Zapamiętana bazowa prędkość (dzięki temu nigdy jej nie zgubimy)
    private float baseSpeed = 8f;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();

        // Zapisujemy prędkość domyślną na starcie, żeby serwer zawsze wiedział, do czego wracać
        if (movement != null)
        {
            baseSpeed = movement.speed;
        }
    }

    [Server]
    public void ApplyBooster(BoosterType type, float duration)
    {
        RpcOnBoosterCollected(type);

        if (type == BoosterType.Speed)
        {
            // Jeśli nie mieliśmy aktywnego boostera, przyspieszamy
            if (speedTimer <= 0f)
            {
                movement.speed = baseSpeed * 2f;
            }

            // Zawsze dodajemy czas (jak podniesiesz dwa boostery, masz 20 sekund zamiast 10!)
            speedTimer += duration;
        }
        else if (type == BoosterType.ScoreMultiplier)
        {
            hasScoreMultiplier = true;
            scoreTimer += duration; // Tu też czas się sumuje
        }
    }

    [ClientRpc]
    void RpcOnBoosterCollected(BoosterType type)
    {
        Debug.Log("ZEBRANO BOOSTER: " + type);
    }

    // ServerCallback sprawia, że tylko Serwer odlicza czas
    [ServerCallback]
    void Update()
    {
        // --- ODLICZANIE CZASU PRĘDKOŚCI ---
        if (speedTimer > 0)
        {
            speedTimer -= Time.deltaTime;

            // Kiedy czas minie, twardo wracamy do prędkości bazowej
            if (speedTimer <= 0)
            {
                speedTimer = 0;
                movement.speed = baseSpeed;
            }
        }

        // --- ODLICZANIE CZASU PUNKTÓW ---
        if (scoreTimer > 0)
        {
            scoreTimer -= Time.deltaTime;

            // Kiedy czas minie, wyłączamy mnożnik
            if (scoreTimer <= 0)
            {
                scoreTimer = 0;
                hasScoreMultiplier = false;
            }
        }
    }
}