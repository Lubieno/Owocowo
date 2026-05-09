using UnityEngine;
using Mirror;

public class FruitCollision : NetworkBehaviour
{
    [SyncVar] public Color fruitColor = Color.blue;
    [SyncVar] public string throwerName = ""; // --- DODANE: Pamięta, kto rzucił ---

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            SheepState sheep = collision.gameObject.GetComponentInParent<SheepState>();

            if (sheep != null)
            {
                // --- ZMIANA: Przekazujemy również nazwę gracza ---
                sheep.ChangeColor(fruitColor, throwerName);
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}