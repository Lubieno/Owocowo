using UnityEngine;

public class FruitCollision : MonoBehaviour
{
    [Header("Ustawienia owocu")]
    public Color playerColor = Color.blue; // Kolor gracza rzucającego
    public float destroyDelay = 2f; // Czas po jakim owoc znika z mapy po rzucie

    void Start()
    {
        // Zniszcz owoc po jakimś czasie, żeby nie zaśmiecać pamięci, jeśli w nic nie trafi
        Destroy(gameObject, destroyDelay);
    }

    // Ta funkcja odpala się automatycznie, gdy owoc (jego Collider) w coś uderzy
    private void OnCollisionEnter(Collision collision)
    {
        // Sprawdzamy, czy uderzony obiekt ma tag "Sheep"
        if (collision.gameObject.CompareTag("Sheep"))
        {
            // Pobieramy komponent renderujący wygląd owcy
            MeshRenderer sheepRenderer = collision.gameObject.GetComponent<MeshRenderer>();

            if (sheepRenderer != null)
            {
                // Zmieniamy kolor materiału na kolor gracza
                sheepRenderer.material.color = playerColor;

                // TODO w przyszłości: Tutaj wyślecie informację do menedżera gry o zdobyciu punktu
            }

            // Niszczymy owoc po trafieniu (rozbryzg!)
            Destroy(gameObject);
        }
    }
}