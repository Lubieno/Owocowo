using UnityEngine;
using UnityEngine.AI;
using Mirror; // DODANO

[RequireComponent(typeof(NavMeshAgent))]
public class SheepWander : NetworkBehaviour // ZMIANA
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
    }

    void Update()
    {
        // KLUCZOWA ZMIANA: Tylko serwer może decydować, gdzie idzie owca. 
        // Klienci będą po prostu oglądać jak idzie, dzięki komponentowi NetworkTransform.
        if (!isServer) return;

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}