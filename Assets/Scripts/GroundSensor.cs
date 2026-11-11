using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    //private bool isGrounded = false;
    [Tooltip("The layer mask that the Raycasts detect in.")]
    public LayerMask raycastMask;
    [Range(0.0f, 5.0f)]
    public float maxRayCastDistance = 3.0f;
    /*private void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, maxRayCastDistance);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }*/

    //public bool IsGrounded => isGrounded;
    public bool IsFacingGround()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, maxRayCastDistance, raycastMask);
    }
}