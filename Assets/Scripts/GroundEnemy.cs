using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundEnemy : Enemy
{
    [Tooltip("The speed of the character moving on the ground.")]
    [Range(0.0f, 10.0f)]
    public float speed = 1.0f;
    [Tooltip("The layer mask that the Raycasts detect in.")]
    public LayerMask raycastMask;
    [Range(0.0f, 3.0f)]
    [Tooltip("The maximum distance of a Raycast check.")]
    public float maxRayCastDistance = 1.0f;

    [SerializeField] private GroundSensor leftGroundSensor;
    [SerializeField] private GroundSensor rightGroundSensor;

    private Rigidbody2D rb;
    public Rigidbody2D Body => rb;

    // The direction that the character is facing (1 is right and -1 is left).
    private int direction = 1;
    public int Direction
    {
        get => direction;
        set
        {
            if (value == 0 || value == direction)
            {
                return;
            }

            direction = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void UpdateLogic()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, maxRayCastDistance, raycastMask) || (leftGroundSensor.IsFacingGround() && !rightGroundSensor.IsFacingGround()))
        {
            direction = -1;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, maxRayCastDistance, raycastMask) || (!leftGroundSensor.IsFacingGround() && rightGroundSensor.IsFacingGround()))
        {
            direction = 1;
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, maxRayCastDistance, raycastMask);
    }
}
