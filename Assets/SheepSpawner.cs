using UnityEngine;

public class SheepSpawner : MonoBehaviour
{
    [Header("Ustawienia spawnu")]
    public GameObject sheepPrefab; // Prefab owcy z podpiętym NavMeshAgent i SheepWander
    public int initialSheepCount = 10; // Ile owiec na start
    public float spawnRadius = 20f; // Promień od środka mapy, gdzie mogą się pojawić

    void Start()
    {
        // Spawnuje początkową ilość owiec przy starcie gry
        for (int i = 0; i < initialSheepCount; i++)
        {
            SpawnSheep();
        }
    }

    public void SpawnSheep()
    {
        // Losuje pozycję w obrębie sfery
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;
        randomPos.y = 0; // Zakładamy płaską mapę na wysokości 0, żeby owce nie respiły się w powietrzu

        // Tworzy owcę
        Instantiate(sheepPrefab, randomPos, Quaternion.identity);
    }

    // Pomocnicza funkcja do rysowania zasięgu spawnu w Edytorze Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
