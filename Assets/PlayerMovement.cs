using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Prędkość bazy (Zsynchronizowana z Boosterami)")]
    [SyncVar] public float speed = 8f;

    [Header("Fizyka w stylu CS:GO")]
    public float gravity = 20f;
    public float jumpForce = 8f;
    public float friction = 6f;
    public float groundAcceleration = 14f;
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
            // ZMIANA: Sprawdzamy czy jesteśmy w Lobby, żeby uwolnić kursor
            if (SceneManager.GetActiveScene().name == "LobbyScene")
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void Update()
    {
        // ZMIANA: Zablokuj całkowicie chodzenie i rozglądanie się TYLKO, jeśli jesteśmy w Lobby!
        if (SceneManager.GetActiveScene().name == "LobbyScene") return;

        if (!isLocalPlayer) return;

        float x = 0f;
        float z = 0f;
        float mouseX = 0f;
        float mouseY = 0f;
        bool jumpPressed = false;

        if (!PauseMenu.isPaused)
        {
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            jumpPressed = Input.GetButton("Jump");
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (playerCamera != null) playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        Vector3 wishDir = transform.right * x + transform.forward * z;
        wishDir.Normalize();

        if (controller.isGrounded) GroundMove(wishDir, jumpPressed);
        else AirMove(wishDir);

        controller.Move(velocity * Time.deltaTime);
    }

    void GroundMove(Vector3 wishDir, bool jumpPressed)
    {
        ApplyFriction();
        Accelerate(wishDir, speed, groundAcceleration);
        velocity.y = -2f;
        if (jumpPressed) velocity.y = jumpForce;
    }

    void AirMove(Vector3 wishDir)
    {
        Accelerate(wishDir, maxAirSpeed, airAcceleration);
        velocity.y -= gravity * Time.deltaTime;
    }

    void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        float currentSpeed = Vector3.Dot(new Vector3(velocity.x, 0, velocity.z), wishDir);
        float addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0) return;

        float accelSpeed = accel * Time.deltaTime * wishSpeed;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        velocity.x += wishDir.x * accelSpeed;
        velocity.z += wishDir.z * accelSpeed;
    }

    void ApplyFriction()
    {
        Vector3 vec = velocity;
        vec.y = 0f;
        float speedMag = vec.magnitude;
        float drop = 0f;

        if (controller.isGrounded)
        {
            float control = speedMag < friction ? friction : speedMag;
            drop = control * friction * Time.deltaTime;
        }

        float newSpeed = speedMag - drop;
        if (newSpeed < 0) newSpeed = 0;
        if (speedMag > 0) newSpeed /= speedMag;

        velocity.x *= newSpeed;
        velocity.z *= newSpeed;
    }
}