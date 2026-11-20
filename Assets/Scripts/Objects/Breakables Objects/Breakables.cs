using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Breakables : MonoBehaviour
{
    BreakableData _breakableData;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            if (!player.isSuperSprinting)
            {
                return;
            }
            else{
                Destroy(this.gameObject);
            }
        }
    }
}
