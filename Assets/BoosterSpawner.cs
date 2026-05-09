using UnityEngine;
using Mirror;

public class BoosterSpawner : NetworkBehaviour
{
    public GameObject boosterPrefab;
    public float spawnInterval = 15f;
    public int boostersAtOnce = 2;
    public float spawnRadius = 20f;

    private float timer;

    [ServerCallback]
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            for (int i = 0; i < boostersAtOnce; i++)
            {
                SpawnBooster();
            }
            timer = 0;
        }
    }

    void SpawnBooster()
    {
        // Losowanie pozycji wewnątrz sfery wokół spawnera
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;
        randomPos.y = 1f; // Ustawiamy na stałej wysokości nad ziemią

        GameObject b = Instantiate(boosterPrefab, randomPos, Quaternion.identity);
        NetworkServer.Spawn(b);
    }
}