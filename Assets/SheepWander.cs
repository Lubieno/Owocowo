using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
public class SheepWander : NetworkBehaviour
{
    [Header("Ustawienia poruszania")]
    public float wanderRadius = 10f;
    public float wanderTimer = 3f;

    [Header("Dźwięki otoczenia")]
    public AudioSource audioSource;
    public AudioClip[] baaSounds; // Tablica, żeby móc dodać kilka różnych wariantów beczenia
    public float minBaaTime = 5f;
    public float maxBaaTime = 15f;

    private float baaTimer;
    private NavMeshAgent agent;
    private float moveTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        moveTimer = wanderTimer;

        // Losujemy czas do pierwszego beczenia
        baaTimer = Random.Range(minBaaTime, maxBaaTime);

        if (!isServer)
        {
            agent.enabled = false;
        }
    }

    void Update()
    {
        if (!isServer) return;

        // --- LOGIKA BECZENIA (Tylko Serwer odlicza i wysyła sygnał) ---
        baaTimer -= Time.deltaTime;
        if (baaTimer <= 0)
        {
            RpcPlayBaaSound(); // Wysyłamy sygnał do wszystkich graczy
            baaTimer = Random.Range(minBaaTime, maxBaaTime); // Losujemy nowy czas
        }

        // --- LOGIKA CHODZENIA ---
        moveTimer += Time.deltaTime;
        if (moveTimer >= wanderTimer)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                if (!float.IsInfinity(newPos.x))
                {
                    agent.SetDestination(newPos);
                }
            }
            moveTimer = 0;
        }
    }

    [ClientRpc]
    void RpcPlayBaaSound()
    {
        // Ta funkcja wykonuje się u każdego klienta, odtwarzając dźwięk
        if (audioSource != null && baaSounds.Length > 0)
        {
            AudioClip randomClip = baaSounds[Random.Range(0, baaSounds.Length)];
            audioSource.pitch = Random.Range(0.85f, 1.15f); // Lekka modulacja głosu
            audioSource.PlayOneShot(randomClip);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
        {
            return navHit.position;
        }
        return origin;
    }
}