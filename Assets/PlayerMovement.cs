using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Ustawienia ruchu")]
    public float speed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f; // NOWE: Zmienna określająca wysokość skoku w metrach

    [Header("Ustawienia kamery (FPS)")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // ==========================================
        // KLUCZOWA ZMIANA SIECIOWA
        // ==========================================
        if (!isLocalPlayer)
        {
            if (playerCamera != null)
            {
                playerCamera.GetComponent<Camera>().enabled = false;
                playerCamera.GetComponent<AudioListener>().enabled = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // 1. Sprawdzenie ziemi na samym początku klatki
        bool groundedPlayer = controller.isGrounded;

        if (groundedPlayer && velocity.y < 0)
        {
            // Ważne: dajemy -2f, żeby postać była "dociskana" do ziemi.
            // Jeśli dasz 0, isGrounded będzie migać (true/false).
            velocity.y = -2f;
        }

        // 2. Rozglądanie się (bez zmian)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (playerCamera != null) playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Obliczanie kierunku chodzenia
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        // 4. Skakanie
        // Sprawdzamy groundedPlayer, które pobraliśmy na początku klatki
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            // Wzór na skok: pierwiastek z (wysokość * -2 * grawitacja)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Aplikowanie grawitacji do prędkości pionowej
        velocity.y += gravity * Time.deltaTime;

        // 6. KLUCZ: Łączymy ruch poziomy i pionowy w JEDEN wykonany ruch
        // To gwarantuje, że grawitacja zawsze "pcha" nas w ziemię, aktualizując isGrounded
        Vector3 finalMove = moveDirection * speed + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }
}
