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
            OldPlayer player = collision.collider.GetComponent<OldPlayer>();
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
