using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Collider2D wallCheckCollider;
    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;

    [Range(0.0f, 50.0f)]
    public float maxGroundSpeed = 20.0f;
    [Range(0.0f, 50.0f)]
    public float maxWallRunSpeed = 30.0f;
    [Range(0.0f, 50.0f)]
    public float maxFallVelocity = 30.0f;
    [Range(0.0f, 100.0f)]
    public float normalAcceleration = 20.0f;
    /*[Range(0.0f, 100.0f)]
    public float superSprintAcceleration = 60.0f;*/
    [Range(0.0f, 50.0f)]
    public float jumpForce = 15.0f;
    [Range(0.0f, 5.0f)]
    public float groundGravityScale = 3.0f;
    [Range(0.0f, 5.0f)]
    public float wallRunGravityScale = 0.0f;


    private bool isGrounded = false;


    private float moveValue = 1.0f;
    private Vector2 currentVelocity;
    public Vector2 Velocity => currentVelocity;
    

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
    private bool isFacingWall = false;
    private bool CanStartWallRun => isSuperSprinting && isReadyToWallRun && isFacingWall;
    private bool isWallRunning = false;
    private bool canWallRunContinue = false;


    private BoxCollider2D playerCollider;
    private Vector2 normalColliderSize;
    private Vector2 normalColliderOffset;
    [Range(0.1f, 1.0f)]
    public float slideColliderHeightMultiplier = 0.5f;
    private bool isSliding;


    [SerializeField] private GameObject attackWeapon;
    private bool canHorizontalAttack = false;
    private bool canAttackDive = false;
    private Vector2 normalAttackWeaponPosition;


    [Range(0.0f, 2.0f)]
    public float superSprintReverseTime = 0.5f;
    private float currentSuperSprintReverseTime;
    private bool isSuperSprintReverseTimeRunning = false;
    private bool isSuperSprinting = false;
    private bool canSuperSprintContinue = false;


    [Range(0.0f, 5.0f)]
    public float coyoteTime = 0.5f;
    private float currentCoyoteTime;
    private bool isCoyoteTimeRunning = false;
    private bool isCoyoteTimeOut = false;



    [Range(0.0f, 5.0f)]
    public float jumpCooldown = 0.8f;
    private float currentJumpCooldownTime;
    private bool isJumpCooldownRunning = false;
    private bool isReadyToJump = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        normalColliderSize = playerCollider.size;
        normalColliderOffset = playerCollider.offset;

        inputActions = new InputSystem_Actions();

        normalAttackWeaponPosition = attackWeapon.transform.localPosition;
        
        currentSuperSprintReverseTime = superSprintReverseTime;
        currentCoyoteTime = coyoteTime;
        currentJumpCooldownTime = jumpCooldown;
    }

    private void OnEnable() {
        inputActions.Enable();
    }
    private void OnDisable() {
        inputActions.Disable();
    }

    private void Start()
    {
        inputActions.Player.Jump.started += context => TriggerJump(context);
        inputActions.Player.Attack.started += context => TriggerHorizontalAttack(context);
        inputActions.Player.AttackDive.started += context => TriggerAttackDive(context);
        inputActions.Player.Attack.canceled += context => CancelHorizontalAttack(context);
        inputActions.Player.Attack.canceled += context => CancelAttackDive(context);
    }

    private void Update()
    {
        ReadInput();

        CheckPlayerOnGround();
        CheckSuperSprint();
        CheckPlayerWallRun();

        CheckPlayerSlide();

        CheckPlayerAttack();

        UpdatePublicVariable();

        CheckJumpCooldown();
        CheckCoyoteTime();
    }

    private void ReadInput() {
        moveValue = inputActions.Player.Move.ReadValue<float>();
        //Set the facing direction of the player
        if (moveValue > 0.0f)
        {
            FacingDirection = 1;
        }
        else if (moveValue < 0.0f)
        {
            FacingDirection = -1;
        }
        
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, moveValue * maxGroundSpeed, normalAcceleration * Time.deltaTime);
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, - maxFallVelocity, Mathf.Infinity);
    }

    private void CheckPlayerWallRun()
    {

        isReadyToWallRun = (inputActions.Player.WallRunLeft.ReadValue<float>() > 0.0f && isFacingWall) || (inputActions.Player.WallRunRight.ReadValue<float>() > 0.0f && isFacingWall);
        Vector2 wallCheckerPosition = wallCheckCollider.transform.localPosition;
        wallCheckerPosition.x = Mathf.Abs(wallCheckerPosition.x) * FacingDirection;
        wallCheckCollider.transform.localPosition = wallCheckerPosition;

        if (CanStartWallRun)
        {
            isWallRunning = true;
            rb.gravityScale = wallRunGravityScale;
        }

        if (isWallRunning)
        {
            canWallRunContinue = true;
            if (!isReadyToWallRun)
            {
                isWallRunning = false;
                rb.gravityScale = groundGravityScale;
                //Can add a jump impulse here
            }

            rb.linearVelocity = new Vector2(0.0f, maxWallRunSpeed);
        }
        else if (canWallRunContinue && isReadyToWallRun)
        {
            isWallRunning = true;
        }
    }

    private void CheckPlayerSlide()
    {
        isSliding = isGrounded && ((inputActions.Player.SlideLeft.ReadValue<float>() > 0.0f) || (inputActions.Player.SlideRight.ReadValue<float>() > 0.0f));
        UpdatePlayerCollider();
    }

    private void UpdatePlayerCollider()
    {
        if (!isWallRunning && isSliding)
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
    private void CheckPlayerOnGround()
    {
        if (isGrounded)
        {
            isReadyToJump = true;
            isCoyoteTimeOut = false;

            canWallRunContinue = false;
        }
    }

    private void UpdatePublicVariable()
    {
        currentVelocity = rb.linearVelocity;
    }

    private void CheckSuperSprint()
    {
        // Suggest adding a grace period so that super sprint lasts a bit longer after collision
        isSuperSprinting = (maxGroundSpeed - Mathf.Abs(rb.linearVelocityX)) < 1e-9;

        if (isSuperSprinting)
        {
            canSuperSprintContinue = true;
        }

        if (isSuperSprintReverseTimeRunning)
        {
            currentSuperSprintReverseTime -= Time.deltaTime;
            if (currentSuperSprintReverseTime <= 0.0f)
            {
                canSuperSprintContinue = false;
                isSuperSprintReverseTimeRunning = false;
                currentSuperSprintReverseTime = superSprintReverseTime;
            }
        }

        if (!isSuperSprinting && canSuperSprintContinue)
        {
            isSuperSprintReverseTimeRunning = true;
        }

        bool isReversingDirection = (moveValue != 0.0f) && (FacingDirection != lastFacingDirection);
        if (isReversingDirection && canSuperSprintContinue)
        {
            Debug.Log("Speed is retained!");
            rb.linearVelocityX = maxGroundSpeed * FacingDirection;
            canSuperSprintContinue = false;
            isSuperSprintReverseTimeRunning = false;
            currentSuperSprintReverseTime = superSprintReverseTime;
        }
    }

    private void CheckPlayerAttack()
    {
        
        if (canHorizontalAttack && (isGrounded || !canAttackDive))
        {
            Vector2 attackWeaponPosition = attackWeapon.transform.localPosition;
            attackWeaponPosition.x = Mathf.Abs(attackWeaponPosition.x) * FacingDirection;
            attackWeapon.transform.localPosition = attackWeaponPosition;
            attackWeapon.SetActive(true);
        }
        else if (!isGrounded && canAttackDive)
        {
            attackWeapon.transform.SetLocalPositionAndRotation(new Vector2(0.0f, - Mathf.Abs(normalAttackWeaponPosition.x)), Quaternion.Euler(0.0f, 0.0f, 90.0f));
            attackWeapon.SetActive(true);
            // Can add a dive impulse here
        }
        else
        {
            attackWeapon.SetActive(false);
            attackWeapon.transform.SetLocalPositionAndRotation(normalAttackWeaponPosition, Quaternion.identity);
        }
    }

    private void CheckJumpCooldown()
    {
        if (isJumpCooldownRunning)
        {
            currentJumpCooldownTime -= Time.deltaTime;
            if (currentJumpCooldownTime <= 0.0f)
            {
                isJumpCooldownRunning = false;
                currentJumpCooldownTime = jumpCooldown;
            }
        }
    }

    private void CheckCoyoteTime()
    {
        if (isJumpCooldownRunning)
        {
            isCoyoteTimeRunning = false;
            currentCoyoteTime = coyoteTime;
            if (isGrounded)
            {
                isJumpCooldownRunning = false;
                currentJumpCooldownTime = jumpCooldown;
            }
        }
        else if (!isGrounded && isReadyToJump && !isCoyoteTimeOut)
        {
            isCoyoteTimeRunning = true;
        }

        if (isCoyoteTimeRunning)
        {
            currentCoyoteTime -= Time.deltaTime;
            if (currentCoyoteTime <= 0.0f)
            {
                isCoyoteTimeRunning = false;
                currentCoyoteTime = coyoteTime;
                isCoyoteTimeOut = true;
            }
        }
    }

    // Action event section
    private void TriggerJump(InputAction.CallbackContext context)
    {
        if (isJumpCooldownRunning || !isReadyToJump)
        {
            return;
        }

        if (isGrounded || isCoyoteTimeRunning)
        {
            float jumpImpulse = rb.mass * (jumpForce - rb.linearVelocityY);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            isJumpCooldownRunning = true;
            isReadyToJump = false;
        }
    }

    private void TriggerHorizontalAttack(InputAction.CallbackContext context)
    {
        Vector2 attackWeaponPosition = attackWeapon.transform.localPosition;
        attackWeaponPosition.x = Mathf.Abs(attackWeaponPosition.x) * FacingDirection;
        attackWeapon.transform.localPosition = attackWeaponPosition;
        attackWeapon.SetActive(true);
        canHorizontalAttack = true;
    }

    private void CancelHorizontalAttack(InputAction.CallbackContext context)
    {
        attackWeapon.SetActive(false);
        canHorizontalAttack = false;
    }

    private void TriggerAttackDive(InputAction.CallbackContext context)
    {
        Vector2 attackWeaponPosition = attackWeapon.transform.localPosition;
        attackWeaponPosition.x = Mathf.Abs(attackWeaponPosition.x) * FacingDirection;
        attackWeapon.transform.localPosition = attackWeaponPosition;
        attackWeapon.SetActive(true);
        canAttackDive = true;
    }

    private void CancelAttackDive(InputAction.CallbackContext context)
    {
        attackWeapon.SetActive(false);
        canAttackDive = false;
    }

    // Trigger delegation section
    public void OnPlayerIsGrounded(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("Ground"))
        {
            Debug.Log("Is grounded!");
            isGrounded = true;
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
}