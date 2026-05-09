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

        if (PauseMenu.isPaused) return;

        if (Input.GetButtonDown("Fire1"))
        {
            // Pobieramy nazwę gracza z profilu
            string myName = "Nieznajomy";
            if (ProfileManager.Instance != null && ProfileManager.Instance.currentProfile != null)
            {
                myName = ProfileManager.Instance.currentProfile.playerName;
            }

            // Sprawdzamy czy mamy booster podwójnych punktów
            PlayerEffects effects = GetComponent<PlayerEffects>();
            bool isDouble = (effects != null && effects.hasScoreMultiplier);

            CmdThrowFruit(fpsCamera.transform.forward, myPlayerColor, myName, isDouble);
        }
    }

    [Command]
    void CmdThrowFruit(Vector3 lookDirection, Color colorToApply, string throwerName, bool isDoublePoints)
    {
        GameObject projectile = Instantiate(fruitPrefab, throwPoint.position, throwPoint.rotation);

        FruitCollision fruitLogic = projectile.GetComponent<FruitCollision>();
        if (fruitLogic != null)
        {
            fruitLogic.fruitColor = colorToApply;
            fruitLogic.throwerName = throwerName;
            fruitLogic.doublePoints = isDoublePoints; // Przekazujemy info o boosterze do pocisku
        }

        NetworkServer.Spawn(projectile);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            Vector3 forceDirection = lookDirection * throwForce;
            forceDirection += Vector3.up * upwardForce;
            rb.linearVelocity = forceDirection;
        }

        StartCoroutine(DestroyProjectileCoroutine(projectile, destroyTime));
    }

    private System.Collections.IEnumerator DestroyProjectileCoroutine(GameObject proj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (proj != null) NetworkServer.Destroy(proj);
    }
}