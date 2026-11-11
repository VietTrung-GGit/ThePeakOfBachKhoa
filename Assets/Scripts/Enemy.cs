using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Tooltip("The speed of the character moving on the ground.")]
    [Range(0.0f, 10.0f)]
    public float speed = 1.0f;
    /*[Tooltip("The acceleration applied when the character falls.")]
    [Range(0.0f, 20.0f)]
    public float gravityAcceleration = 10.0f;*/
    [Tooltip("The layer mask that the Raycasts detect in.")]
    public LayerMask raycastMask;
    [Range(0.0f, 3.0f)]
    [Tooltip("The maximum distance of a Raycast check.")]
    public float maxRayCastDistance = 1.0f;
    /*[Range(0.0f, 10.0f)]
    [Tooltip("The magnitude of the knockback force when the character collides with another character.")]
    public float knockbackForceMagnitude = 1.0f;*/
    [Range(0.0f, 5.0f)]
    [Tooltip("The time the character must wait to escape the threatened state.")]
    public float threatTime = 3.0f;
    [Range(0.0f, 5.0f)]
    [Tooltip("The time the character must wait to escape the paralyzed state.")]
    public float paralysisTime = 3.0f;
    private float currentThreatTime;
    private float currentParalysisTime;

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

    // Check if the player is threatening this character.
    private bool isThreatened = false;
    // Check if the player poses a threat to this character.
    private bool isPlayerAThreat = false;
    private bool isParalyzed = false;
    public bool IsThreatened => isThreatened;
    public bool IsParalyzed => isParalyzed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentParalysisTime = paralysisTime;
        currentThreatTime = threatTime;
    }

    private void Update()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, maxRayCastDistance, raycastMask) || (leftGroundSensor.IsFacingGround() && !rightGroundSensor.IsFacingGround()))
        {
            direction = -1;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, maxRayCastDistance, raycastMask) || (!leftGroundSensor.IsFacingGround() && rightGroundSensor.IsFacingGround()))
        {
            direction = 1;
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (isThreatened && !isPlayerAThreat)
        {
            currentThreatTime -= Time.deltaTime;
            if (currentThreatTime <= 0.0f)
            {
                isThreatened = false;
            }
        }

        if (isParalyzed)
        {
            currentParalysisTime -= Time.deltaTime;
            if (currentParalysisTime <= 0.0f)
            {
                isParalyzed = false;
            }
        }
    }
    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, maxRayCastDistance, raycastMask);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Must check if Player is in Super Sprint
        if (collision.CompareTag("Player"))
        {
            currentThreatTime = threatTime;
            isThreatened = true;
            isPlayerAThreat = true;
        }
    }

    //Add a OnTriggerStay2D to check if Player is in Super Sprint

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //isThreatened = false;
            isPlayerAThreat = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Knock back and defeat based on collision with player and whether the player is in Super Sprint
        if (collision.collider.CompareTag("Player"))
        {
            //gameObject.SetActive(false);
            /*Vector2 knockbackForce = new Vector2(knockbackForceMagnitude * -direction, knockbackForceMagnitude);
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);*/
            currentParalysisTime = paralysisTime;
            isParalyzed = true;
            //StartCoroutine(WaitForParalysis(paralysisTime));
            //rb.AddForceX(knockbackForceMagnitude * -direction, ForceMode2D.Impulse);
        }
    }
}
