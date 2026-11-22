using Unity.VisualScripting;
using UnityEngine;

public class MassOrb : Collectible
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]  private  MassOrbData massOrbData;
    void OnPickUp(Player player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.mass = massOrbData.massMulti() * rb.mass;
    }
}
