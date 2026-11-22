using Unity.VisualScripting;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickUp(other.GetComponent<Player>());
            Destroy(gameObject);
        }
    }
    void OnPickUp(Player player)
    {
        Debug.Log("Collectible picked up");
    }
}
