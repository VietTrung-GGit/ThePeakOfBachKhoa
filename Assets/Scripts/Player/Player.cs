using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D PlayerRigidbody => rb;
    private InputSystem_Actions inputActions;

    [Header("Physics Properties")]
    [SerializeField] private Collider2D wallCheckCollider;
    
    [Range(0.0f, 50.0f)]
    public float maxGroundSpeed = 20.0f;
    [Range(0.0f, 50.0f)]
    public float maxWallRunSpeed = 30.0f;
    [Range(0.0f, 50.0f)]
    public float maxFallVelocity = 30.0f;
    [Range(0.0f, 100.0f)]
    public float groundAcceleration = 20.0f;
    [Range(0.0f, 100.0f)]
    public float fallAcceleration = 20.0f;
    [Range(0.0f, 50.0f)]
    public float jumpForce = 15.0f;
    [Range(0.0f, 50.0f)]
    public float knockbackForce = 15.0f;
    [Range(0.0f, 5.0f)]
    public float groundGravityScale = 3.0f;
    [Range(0.0f, 5.0f)]
    public float wallRunGravityScale = 0.0f;


    private bool isGrounded = false;
    public bool IsGrounded => isGrounded;
    private bool isFalling = false;
    public bool IsFalling => isFalling;


    private float moveValue = 1.0f;
    public float MoveValue => moveValue;
    

    private int lastFacingDirection = 1;
    private int facingDirection = 1;
    public int FacingDirection
    {
        get => facingDirection;
        private set
        {
            lastFacingDirection = facingDirection;
            facingDirection = value > 0? 1 : -1;
        }
    }


    private bool isReadyToWallRun = false;
    public bool IsReadyToWallRun => isReadyToWallRun;
    private bool isFacingWall = false;
    public bool CanStartWallRun => isSuperSprinting && isReadyToWallRun;
    private bool canWallRunContinue = false;
    public bool CanWallRunContinue
    {
        get => canWallRunContinue;
        set => canWallRunContinue = value;
    }
    public bool CanRestartWallRun => canWallRunContinue && isReadyToWallRun;


    private BoxCollider2D playerCollider;
    private Vector2 normalColliderSize;
    private Vector2 normalColliderOffset;

    [Header("Slide Properties")]
    [Range(0.1f, 1.0f)]
    public float slideColliderHeightMultiplier = 0.5f;
    private bool isSliding;
    public bool IsSliding => isSliding;


    [Header("Run Properties")]
    /*[Range(0.0f, 2.0f)]
    public float superSprintReverseTime = 0.5f;*/
    [Range(0.0f, 2.0f)]
    public float superSprintGraceTime = 0.5f;
    private bool isSuperSprinting = false;
    public bool IsSuperSprinting
    {
        get => isSuperSprinting;
        set => isSuperSprinting = value;
    }
    
    private bool isReversingDirection = false;
    public bool IsReversingDirection => isReversingDirection;

    [Header("Jump Properties")]
    [Range(0.0f, 5.0f)]
    public float coyoteTime = 0.5f;


    private bool canCoyoteJump = false;
    public bool CanCoyoteJump
    {
        get => canCoyoteJump;
        set => canCoyoteJump = value;
    }
    private bool isReadyToJump = false;
    public bool IsReadyToJump => isReadyToJump;


    private bool isHurt = false;
    public bool IsHurt
    {
        get => isHurt;
        set => isHurt = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        normalColliderSize = playerCollider.size;
        normalColliderOffset = playerCollider.offset;

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable() {
        inputActions.Player.Enable();
    }
    private void OnDisable() {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        ReadMoveInput();

        CheckPlayerFall();

        CheckSuperSprint();

        CheckPlayerWallRun();

        CheckPlayerSlide();

        CheckPlayerJump();
    }

    private void ReadMoveInput() {
        moveValue = inputActions.Player.Move.ReadValue<float>();

        UpdateFacingDirection();

        //rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, moveValue * maxGroundSpeed, groundAcceleration * Time.deltaTime);
        //rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, - maxFallVelocity, Mathf.Infinity);
    }

    private void UpdateFacingDirection()
    {
        if (moveValue > 0.0f)
        {
            FacingDirection = 1;
        }
        else if (moveValue < 0.0f)
        {
            FacingDirection = -1;
        }
    }

    private void CheckPlayerFall()
    {
        isFalling = !isGrounded && rb.linearVelocityY <= 0.0f;
    }

    private void CheckSuperSprint()
    {
        //isSuperSprinting = (maxGroundSpeed - Mathf.Abs(rb.linearVelocityX)) < 1e-9;
        if ((maxGroundSpeed - Mathf.Abs(rb.linearVelocityX)) < 1e-9)
        {
            isSuperSprinting = true;
        }

        isReversingDirection = (moveValue != 0.0f) && (FacingDirection != lastFacingDirection);
    }

    private void CheckPlayerWallRun()
    {

        isReadyToWallRun = (inputActions.Player.WallRunLeft.ReadValue<float>() > 0.0f && isFacingWall) || (inputActions.Player.WallRunRight.ReadValue<float>() > 0.0f && isFacingWall);
        Vector2 wallCheckerPosition = wallCheckCollider.transform.localPosition;
        wallCheckerPosition.x = Mathf.Abs(wallCheckerPosition.x) * FacingDirection;
        wallCheckCollider.transform.localPosition = wallCheckerPosition;
    }

    private void CheckPlayerSlide()
    {
        isSliding = isGrounded && ((inputActions.Player.SlideLeft.ReadValue<float>() > 0.0f) || (inputActions.Player.SlideRight.ReadValue<float>() > 0.0f));
    }

    public void UpdatePlayerCollider(bool isColliderLower)
    {
        if (isColliderLower)
        {
            float newColliderHeight = normalColliderSize.y * slideColliderHeightMultiplier;
            playerCollider.size = new Vector2(normalColliderSize.x, newColliderHeight);
            playerCollider.offset = new Vector2(normalColliderOffset.x, normalColliderOffset.y - (normalColliderSize.y - newColliderHeight) / 2.0f);
        }
        else
        {
            playerCollider.size = normalColliderSize;
            playerCollider.offset = normalColliderOffset;
        }
    }

    private void CheckPlayerJump()
    {
        isReadyToJump = inputActions.Player.Jump.ReadValue<float>() > 0.0f;
    }


    // Trigger delegation section
    public void OnPlayerIsGrounded(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("Ground"))
        {
            Debug.Log("Is grounded!");
            isGrounded = true;

            canCoyoteJump = true;
            canWallRunContinue = false;
        }
    }

    public void OnPlayerIsAirborne(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("Ground"))
        {
            Debug.Log("Is not grounded");
            isGrounded = false;
        }
    }

    public void OnPlayerFacingWall(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("Ground"))
        {
            isFacingWall = true;
        }
    }

    public void OnPlayerNotFacingWall(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("Ground"))
        {
            isFacingWall = false;
        }
    }

    public void OnPlayerGetHurt(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("HurtPlayer"))
        {
            isHurt = true;
        }
    }
}