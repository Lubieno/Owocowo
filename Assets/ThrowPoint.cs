using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    [Header("Ustawienia rzutu")]
    public GameObject fruitPrefab;
    public Transform throwPoint;
    public Camera fpsCamera;
    public float throwForce = 15f;
    public float upwardForce = 2f;
    public float destroyTime = 10f; // NOWE: Zmienna określająca czas życia piłki

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ThrowFruit();
        }
    }

    void ThrowFruit()
    {
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 directionToTarget = targetPoint - throwPoint.position;

        // 3. Stworzenie owocu
        GameObject projectile = Instantiate(fruitPrefab, throwPoint.position, throwPoint.rotation);

        // --- KLUCZOWA ZMIANA ---
        // Ta linijka nakazuje Unity usunąć piłkę po 10 sekundach od jej stworzenia
        Destroy(projectile, destroyTime);
        // -----------------------

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forceDirection = directionToTarget.normalized * throwForce;
            forceDirection += Vector3.up * upwardForce;

            rb.AddForce(forceDirection, ForceMode.Impulse);

            projectile.transform.forward = directionToTarget.normalized;
        }
        string throwData = "THROW|" + throwPoint.position.ToString() + "|" + directionToTarget.ToString();
        FindObjectOfType<UDPNetworkManager>().SendData(throwData);
    }
}
