using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float speed = 8f;
    public float gravity = -9.81f;
    
    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        // Pobieramy komponent podpięty pod ten sam obiekt
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Pobieranie wejścia z klawiatury (W, A, S, D lub strzałki)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Obliczanie kierunku ruchu względem tego, gdzie patrzy gracz
        Vector3 move = transform.right * x + transform.forward * z;

        // Aplikowanie ruchu
        controller.Move(move * speed * Time.deltaTime);

        // Prosta grawitacja, żeby gracz "trzymał" się ziemi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}