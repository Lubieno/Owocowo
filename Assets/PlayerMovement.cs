using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Ustawienia ruchu")]
    public float speed = 8f;
    public float gravity = -9.81f;

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
            // Jeśli to "cudza" postać wyłączamy jej kamerę i uszy
            if (playerCamera != null)
            {
                playerCamera.GetComponent<Camera>().enabled = false;
                playerCamera.GetComponent<AudioListener>().enabled = false;
            }
        }
        else
        {
            // Tylko na naszym komputerze blokujemy kursor
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        
        // ==========================================
        // 1. ROZGLĄDANIE SIĘ (MYSZKA)
        // ==========================================
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);

        // ==========================================
        // 2. CHODZENIE (KLAWIATURA)
        // ==========================================
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}