using UnityEngine;
using Mirror;

public class FruitCollision : NetworkBehaviour
{
    [SyncVar] public Color fruitColor = Color.blue;
    [SyncVar] public string throwerName = "";
    [SyncVar] public bool doublePoints = false; // Czy ten owoc daje x2 pkt

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            SheepState sheep = collision.gameObject.GetComponentInParent<SheepState>();

            if (sheep != null)
            {
                // Przekazujemy kolor, nazwę gracza oraz informację o boosterze
                sheep.ChangeColor(fruitColor, throwerName, doublePoints);
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}