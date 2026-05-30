using UnityEngine;
using Mirror;

public class SheepSpawner : NetworkBehaviour
{
    public GameObject sheepPrefab;
    public int initialSheepCount = 10;
    public float spawnRadius = 20f;

    public override void OnStartServer()
    {
        for (int i = 0; i < initialSheepCount; i++)
        {
            SpawnSheep();
        }
    }

    public void SpawnSheep()
    {
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;
        randomPos.y = 0;

        GameObject spawnedSheep = Instantiate(sheepPrefab, randomPos, Quaternion.identity);
        NetworkServer.Spawn(spawnedSheep);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}