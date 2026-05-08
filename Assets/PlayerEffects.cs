using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerEffects : NetworkBehaviour
{
    private PlayerMovement movement;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    [Server] // Tylko serwer zarządza stanem logicznym
    public void ApplyBooster(BoosterType type, float duration)
    {
        // Wywołaj funkcję na kliencie, aby wiedział o zmianie (np. UI)
        RpcOnBoosterCollected(type);

        // Rozpocznij efekt na serwerze (aby zsynchronizować zmiany w ruchu)
        StartCoroutine(BoosterRoutine(type, duration));
    }

    [ClientRpc]
    void RpcOnBoosterCollected(BoosterType type)
    {
        Debug.Log("Zebrałeś booster: " + type);
        // Tutaj możesz dodać dźwięk lub ikonkę w UI
    }

    IEnumerator BoosterRoutine(BoosterType type, float duration)
    {
        switch (type)
        {
            case BoosterType.Speed:
                float originalSpeed = movement.speed;
                movement.speed *= 2f; // Zmiana synchronizowana przez NetworkTransform gracza
                yield return new WaitForSeconds(duration);
                movement.speed = originalSpeed;
                break;

            case BoosterType.DoubleJump:
                // Załóżmy, że w PlayerMovement masz zmienną canDoubleJump
                // movement.canDoubleJump = true;
                yield return new WaitForSeconds(duration);
                // movement.canDoubleJump = false;
                break;
        }
    }
}
