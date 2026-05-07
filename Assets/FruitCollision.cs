using UnityEngine;
using Mirror;

public class FruitCollision : NetworkBehaviour 
{
    [Header("Ustawienia owocu")]
    // Synchronizujemy kolor, żeby klienci widzieli lecącą kolorową piłkę
    [SyncVar] public Color fruitColor = Color.blue;

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            // ZMIANA: Szukamy skryptu na uderzonym obiekcie LUB jego rodzicu
            // To kluczowe, gdy owoc trafi w model z Blendera (dziecko)
            SheepState sheep = collision.gameObject.GetComponentInParent<SheepState>();

            if (sheep != null)
            {
                Debug.Log("[SERWER] Trafiono owcę, zmieniam kolor!");
                sheep.ChangeColor(fruitColor);
            }
            else
            {
                Debug.LogWarning("[SERWER] Trafiono obiekt z tagiem Sheep, ale nie znaleziono SheepState!");
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}