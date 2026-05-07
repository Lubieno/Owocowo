using UnityEngine;
using Mirror;

public class PlayerThrow : NetworkBehaviour
{
    [Header("Ustawienia gracza")]
    public Color myPlayerColor = Color.blue;

    [Header("Ustawienia rzutu")]
    public GameObject fruitPrefab;
    public Transform throwPoint;
    public Camera fpsCamera;
    public float throwForce = 15f;
    public float upwardForce = 2f;
    public float destroyTime = 10f;

    void Start()
    {
        // Skrypt sam szuka punktu rzutu, tak jak zrobiliśmy to przed chwilą
        if (throwPoint == null)
        {
            throwPoint = transform.Find("MIEJSCE_RZUTU");
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetButtonDown("Fire1"))
        {
            // KLUCZOWA ZMIANA: Zamiast strzelać laserem (który trafiał w gracza), 
            // pobieramy po prostu wektor przodu naszej kamery!
            CmdThrowFruit(fpsCamera.transform.forward, myPlayerColor);
        }
    }

    [Command]
    void CmdThrowFruit(Vector3 lookDirection, Color colorToApply)
    {
        // 1. Zespawnuj obiekt
        GameObject projectile = Instantiate(fruitPrefab, throwPoint.position, throwPoint.rotation);

        // 2. Ustaw kolor
        FruitCollision fruitLogic = projectile.GetComponent<FruitCollision>();
        if (fruitLogic != null)
        {
            fruitLogic.fruitColor = colorToApply;
        }

        // 3. NAJPIERW pojawiamy pocisk w sieci
        NetworkServer.Spawn(projectile);

        // 4. POTEM nadajemy mu fizykę na serwerze
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            // Nadajemy prędkość w kierunku patrzenia kamery + lekko w górę
            Vector3 forceDirection = lookDirection * throwForce;
            forceDirection += Vector3.up * upwardForce;

            rb.linearVelocity = forceDirection;
        }

        // 5. Uruchamiamy odliczanie do zniszczenia pocisku
        StartCoroutine(DestroyProjectileCoroutine(projectile, destroyTime));
    }

    private System.Collections.IEnumerator DestroyProjectileCoroutine(GameObject proj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (proj != null)
        {
            NetworkServer.Destroy(proj);
        }
    }
}