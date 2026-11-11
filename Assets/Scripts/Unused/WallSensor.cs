using UnityEngine;

public class WallSensor : MonoBehaviour
{
    private bool isBlocked = false;

    [Range(0.0f, 3.0f)]
    public float maxRayCastDistance = 3.0f;

    public bool facingRight = false;
    private void Update()
    {
        isBlocked = Physics2D.Raycast(transform.position, facingRight? Vector2.right : Vector2.left, maxRayCastDistance);
        Debug.DrawRay(transform.position, facingRight? Vector2.right : Vector2.left, Color.red);
    }

    public bool IsBlocked => isBlocked;
}