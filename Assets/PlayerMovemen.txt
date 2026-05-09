using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Ustawienia ruchu")]
    [SyncVar] public float speed = 8f; // Zsynchronizowane do boosterów
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Ustawienia kamery (FPS)")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

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

        // 1. Sprawdzenie ziemi
        bool groundedPlayer = controller.isGrounded;
        if (groundedPlayer && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Zmienne sterujące - domyślnie 0 (brak ruchu)
        float x = 0f;
        float z = 0f;
        float mouseX = 0f;
        float mouseY = 0f;
        bool jumpPressed = false;

        // --- KLUCZOWA ZMIANA ---
        // Zczytujemy klawiaturę i myszkę TYLKO wtedy, gdy menu NIE jest zapauzowane
        if (!PauseMenu.isPaused)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            jumpPressed = Input.GetButtonDown("Jump");
        }

        // 2. Rozglądanie się (jeśli zapauzowane, mouseX i mouseY wynoszą 0, więc kamera stoi)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (playerCamera != null) playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Obliczanie kierunku chodzenia (jeśli zapauzowane, x i z wynoszą 0, więc nie idziemy)
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        // 4. Skakanie
        if (jumpPressed && groundedPlayer)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Aplikowanie grawitacji (DZIAŁA ZAWSZE, NAWET W MENU!)
        velocity.y += gravity * Time.deltaTime;

        // 6. Wykonanie ruchu (DZIAŁA ZAWSZE, uziemiając gracza)
        Vector3 finalMove = moveDirection * speed + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }
}