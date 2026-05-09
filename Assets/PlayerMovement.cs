using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Prędkość bazy (Zsynchronizowana z Boosterami)")]
    [SyncVar] public float speed = 8f;

    [Header("Fizyka w stylu CS:GO")]
    public float gravity = 20f;
    public float jumpForce = 8f;
    public float friction = 6f; // Tarcie na ziemi
    public float groundAcceleration = 14f;

    // Magia Bhopa - te dwie wartości decydują o airstrafingu!
    public float airAcceleration = 2000f;
    public float maxAirSpeed = 2f;

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

        float x = 0f;
        float z = 0f;
        float mouseX = 0f;
        float mouseY = 0f;
        bool jumpPressed = false;

        if (!PauseMenu.isPaused)
        {
            // UWAGA: Używamy GetAxisRaw zamiast GetAxis, żeby usunąć sztuczne opóźnienie klawiatury Unity
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");

            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Auto-Bhop (Trzymając spację, skaczesz od razu). 
            // Jeśli chcesz trudnego, klasycznego bhopa z CS'a, zmień GetButton na GetButtonDown!
            jumpPressed = Input.GetButton("Jump");
        }

        // Rozglądanie się kamery
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (playerCamera != null) playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Chciany kierunek ruchu (WishDir)
        Vector3 wishDir = transform.right * x + transform.forward * z;
        wishDir.Normalize();

        // Serce silnika fizycznego - podział na ziemię i powietrze
        if (controller.isGrounded)
        {
            GroundMove(wishDir, jumpPressed);
        }
        else
        {
            AirMove(wishDir);
        }

        // Fizyczne przesunięcie postaci
        controller.Move(velocity * Time.deltaTime);
    }

    // --- FIZYKA ZIEMI ---
    void GroundMove(Vector3 wishDir, bool jumpPressed)
    {
        ApplyFriction(); // Tarcie działa tylko na ziemi

        // Zwiększamy prędkość do wartości "speed" (limit, który boostery mogą zmieniać)
        Accelerate(wishDir, speed, groundAcceleration);

        // Lekkie dociskanie do ziemi, żeby nie skakać na mikroskopijnych nierównościach
        velocity.y = -2f;

        if (jumpPressed)
        {
            velocity.y = jumpForce; // Wybicie w powietrze!
        }
    }

    // --- FIZYKA POWIETRZA ---
    void AirMove(Vector3 wishDir)
    {
        // W powietrzu używamy ogromnego przyspieszenia (airAcceleration), 
        // ale bardzo małego limitu bezpośredniej prędkości z klawiszy (maxAirSpeed).
        // To zmusza gracza do skręcania kamerą, aby budować prędkość!
        Accelerate(wishDir, maxAirSpeed, airAcceleration);

        // Grawitacja
        velocity.y -= gravity * Time.deltaTime;
    }

    // --- MATEMATYKA SILNIKA SOURCE (Quake / CS:GO) ---
    void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        // Sprawdzamy, ile prędkości już mamy w chcianym kierunku
        float currentSpeed = Vector3.Dot(new Vector3(velocity.x, 0, velocity.z), wishDir);

        // Ile brakuje nam do limitu?
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0) return; // Jeśli przekraczamy limit w tym kierunku, nie dodajemy sztucznie więcej

        // Obliczamy przyspieszenie dla tej klatki
        float accelSpeed = accel * Time.deltaTime * wishSpeed;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        // Dodajemy wyliczoną prędkość do głównego wektora
        velocity.x += wishDir.x * accelSpeed;
        velocity.z += wishDir.z * accelSpeed;
    }

    void ApplyFriction()
    {
        Vector3 vec = velocity;
        vec.y = 0f; // Tarcie nie wpływa na spadanie
        float speedMag = vec.magnitude;
        float drop = 0f;

        // Im szybciej biegniesz, tym mocniej działa tarcie
        if (controller.isGrounded)
        {
            float control = speedMag < friction ? friction : speedMag;
            drop = control * friction * Time.deltaTime;
        }

        float newSpeed = speedMag - drop;
        if (newSpeed < 0) newSpeed = 0;
        if (speedMag > 0) newSpeed /= speedMag;

        // Skalowanie wektora po odjęciu tarcia
        velocity.x *= newSpeed;
        velocity.z *= newSpeed;
    }
}