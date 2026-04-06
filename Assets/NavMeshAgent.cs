using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SheepWander : MonoBehaviour
{
    [Header("Ustawienia poruszania")]
    public float wanderRadius = 10f; // Jak daleko może odejść
    public float wanderTimer = 3f;   // Co ile sekund zmienia kierunek

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            // Wyznaczamy nowy losowy punkt na mapie
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

            // Wysyłamy tam owcę
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    // Funkcja pomocnicza: znajduje losowy, prawidłowy punkt na NavMeshu
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        // Sprawdza, czy losowy punkt faktycznie leży na powierzchni podłogi (NavMesh)
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
