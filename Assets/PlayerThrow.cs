using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerThrow : NetworkBehaviour
{
    [Header("Ustawienia gracza (Awaryjne)")]
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
        if (throwPoint == null) throwPoint = transform.Find("MIEJSCE_RZUTU");
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "LobbyScene") return;
        if (!isLocalPlayer) return;
        if (PauseMenu.isPaused) return;

        if (Input.GetButtonDown("Fire1"))
        {
            LobbyPlayer myLobbyData = GetComponent<LobbyPlayer>();

            string myName = myLobbyData != null ? myLobbyData.playerName : "Nieznajomy";
            Color myColor = myLobbyData != null ? myLobbyData.playerColor : myPlayerColor;

            // Log testowy - zobacz w konsoli, co tu się wyświetli podczas strzelania!
            Debug.Log($"Rzucam owoc! Mój wybrany kolor to: {myColor}");

            PlayerEffects effects = GetComponent<PlayerEffects>();
            bool isDouble = (effects != null && effects.hasScoreMultiplier);

            CmdThrowFruit(fpsCamera.transform.forward, myColor, myName, isDouble);
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
            fruitLogic.doublePoints = isDoublePoints;
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