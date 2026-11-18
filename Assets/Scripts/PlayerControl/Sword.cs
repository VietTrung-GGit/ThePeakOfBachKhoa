using UnityEngine;
public class Sword : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("WeakBlock")) Destroy(other.gameObject);
    }
}