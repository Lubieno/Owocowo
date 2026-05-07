using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
public class SheepWander : NetworkBehaviour 
{
    [Header("Ustawienia poruszania")]
    public float wanderRadius = 10f;
    public float wanderTimer = 3f;

    private NavMeshAgent agent;
    private float timer;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		timer = wanderTimer;

		// === KLUCZOWY DODATEK MULTIPLAYER ===
		// Jeśli ten skrypt uruchamia się u "Zwykłego Gracza" (Klienta), a nie na Serwerze/Hoście:
		if (!isServer)
		{
			// Wyłączamy sztuczną inteligencję. U klientów owca to tylko "kukła", 
			// którą przesuwa NetworkTransform na podstawie danych z serwera!
			agent.enabled = false;
		}
	}

    void Update()
    {
        // Tylko serwer zarządza ruchem
        if (!isServer) return;

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            // 1. Sprawdzamy, czy agent jest aktywny i czy stoi na NavMeshu
            if (agent != null && agent.isOnNavMesh)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                
                // 2. ZABEZPIECZENIE: Sprawdzamy, czy wylosowany punkt jest prawidłowy
                // Point.x == Infinity to najczęstszy powód błędu w konsoli
                if (!float.IsInfinity(newPos.x))
                {
                    agent.SetDestination(newPos);
                }
            }
            
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        // Szukamy najbliższego punktu na NavMeshu w promieniu 'dist'
        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
        {
            return navHit.position;
        }

        // Jeśli nie znaleziono punktu, zwracamy obecną pozycję (żeby nie było infinity)
        return origin;
    }
}