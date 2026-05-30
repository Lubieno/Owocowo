using UnityEngine;
using Mirror;

public class FruitCollision : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorReady))] public Color fruitColor;
    [SyncVar] public string throwerName = "";
    [SyncVar] public bool doublePoints = false;

    public override void OnStartClient()
    {
        ApplyColorToFruit(fruitColor);
    }

    void OnColorReady(Color oldCol, Color newCol)
    {
        ApplyColorToFruit(newCol);
    }

    void ApplyColorToFruit(Color c)
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            if (r != null)
            {
                // ZMIANA DLA URP: Używamy SetColor z dopiskiem "_BaseColor"
                r.material.SetColor("_BaseColor", c);
            }
        }
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            SheepState sheep = collision.gameObject.GetComponentInParent<SheepState>();

            if (sheep != null)
            {
                sheep.ChangeColor(fruitColor, throwerName, doublePoints);
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}