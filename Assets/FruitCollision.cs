using UnityEngine;
using Mirror; // DODANO: Obsługa sieci

public class FruitCollision : NetworkBehaviour // ZMIANA: Z MonoBehaviour na NetworkBehaviour
{
    [Header("Ustawienia owocu")]
    // Ten kolor będzie nadany przez gracza rzucającego
    public Color fruitColor = Color.blue;

    // [ServerCallback] sprawia, że fizyka i kolizje (z punktu widzenia gry logicznej) 
    // są przeliczane TYLKO na serwerze. Zapobiega to podwójnym trafieniom.
    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            // Szukamy naszego nowego skryptu na owcy
            SheepState sheep = collision.gameObject.GetComponent<SheepState>();

            if (sheep != null)
            {
                // Mówimy owcy, żeby przyjęła kolor owocu
                sheep.ChangeColor(fruitColor);
            }

            // Ważne: W sieci używamy NetworkServer.Destroy, a nie zwykłego Destroy!
            NetworkServer.Destroy(gameObject);
        }
    }
}