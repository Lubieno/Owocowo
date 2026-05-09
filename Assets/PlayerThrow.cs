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
            // --- ZMIANA: Pobieramy nazwę gracza z systemu Profili ---
            string myName = "Nieznajomy";
            if (ProfileManager.Instance != null && ProfileManager.Instance.currentProfile != null)
            {
                myName = ProfileManager.Instance.currentProfile.playerName;
            }

            CmdThrowFruit(fpsCamera.transform.forward, myPlayerColor, myName);
        }
    }

    [Command]
    void CmdThrowFruit(Vector3 lookDirection, Color colorToApply, string throwerName)
    {
        GameObject projectile = Instantiate(fruitPrefab, throwPoint.position, throwPoint.rotation);

        FruitCollision fruitLogic = projectile.GetComponent<FruitCollision>();
        if (fruitLogic != null)
        {
            fruitLogic.fruitColor = colorToApply;
            // --- ZMIANA: Przekazujemy nazwę rzucającego do owocu ---
            fruitLogic.throwerName = throwerName;
        }

        NetworkServer.Spawn(projectile);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            Vector3 forceDirection = lookDirection * throwForce;
            forceDirection += Vector3.up * upwardForce;
            rb.linearVelocity = forceDirection; // używam standardowego .velocity dla starszych wersji, ale linearVelocity jest super w Unity 6
        }

        StartCoroutine(DestroyProjectileCoroutine(projectile, destroyTime));
    }

    private System.Collections.IEnumerator DestroyProjectileCoroutine(GameObject proj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (proj != null) NetworkServer.Destroy(proj);
    }
}