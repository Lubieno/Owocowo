using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float speed = 8f;
    public float gravity = -9.81f;

    [Header("Ustawienia kamery (FPS)")]
    public float mouseSensitivity = 2f; // Zmniejszona czułość, aby pasowała do braku Time.deltaTime
    public Transform playerCamera;      // Pamiętaj, aby mieć tu przypisaną Main Camera w Inspektorze

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        // Pobieramy komponent odpowiedzialny za ruch z tego samego obiektu
        controller = GetComponent<CharacterController>();

        // Ukrywamy i blokujemy kursor na środku ekranu
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ==========================================
        // 1. ROZGLĄDANIE SIĘ (MYSZKA)
        // ==========================================

        // Pobieranie ruchu myszki (bez Time.deltaTime dla idealnej płynności)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Obliczanie obrotu w górę i w dół (oś X)
        xRotation -= mouseY;
        // Ograniczenie obrotu, żeby gracz nie zrobił salta w tył (od -90 do 90 stopni)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Aplikowanie obrotu góra/dół TYLKO do kamery
        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Aplikowanie obrotu lewo/prawo (oś Y) do CAŁEJ postaci gracza
        transform.Rotate(Vector3.up * mouseX);


        // ==========================================
        // 2. CHODZENIE (KLAWIATURA)
        // ==========================================

        // Pobieranie wejścia z klawiatury (W, A, S, D)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Obliczanie kierunku ruchu względem tego, gdzie patrzy gracz
        Vector3 move = transform.right * x + transform.forward * z;

        // Aplikowanie ruchu w poziomie
        controller.Move(move * speed * Time.deltaTime);

        // Aplikowanie grawitacji, żeby gracz nie unosił się w powietrzu
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
