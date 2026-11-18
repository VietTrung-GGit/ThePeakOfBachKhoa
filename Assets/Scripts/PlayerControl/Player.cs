using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction wallRunAction;
    private InputAction attackAction;
    public bool slideInputHold;
    public bool AttackRequested { get; private set; }
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private int moveInput;
    public int MoveInput => moveInput;
    private int lastMoveInput;
    public int facingDirection = 1;
    public bool isSuperSprinting;
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    public bool jumpRequested;
    [SerializeField, Min(0f)] private float coyoteTime = 0.10f;
    private float lastGroundedTime = float.NegativeInfinity;
    [Header("Fall")]
    [SerializeField] private float maxFallVelocity = 18f;
    [Header("CheckCollision")]
    [SerializeField] private Vector2 sizeWall;
    [SerializeField] private Transform wallCheck;
    public LayerMask WhatIsGround;
    [SerializeField] private float GroundCheckDistance;
    private bool WallFound;
    public bool isGrounded;
    [Header("SlideInfo")]
    [SerializeField] private float slideTimer;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideCoolDown;
    [SerializeField] private float DistanceFromHeadToCeiling;
    [HideInInspector] public float slideCoolDownTimer;
    private float slideTimerCount;
    public bool isSliding;
    private bool foundCeilingOnHead;
    private bool lastSlideCombo;

    [Header("Wall Running")]
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float yWhenChangeDirection;
    [SerializeField] private float wallRunCooldown = 0.2f;
    private float wallRunCooldownTimer;
    public bool isWallRunning;
    public bool holdWall;
    private int wallDirection;
    private float oldGravity;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        oldGravity = rb.gravityScale;
        if (playerControls == null) {
            Debug.LogError("PlayerControls Asset is not assigned!");
            return;
        }
        playerActionMap = playerControls.FindActionMap("Player");

        if (playerActionMap == null) {
            Debug.LogError("Player Action Map not found!");
            return;
        }
        moveAction = playerActionMap.FindAction("Move");
        jumpAction = playerActionMap.FindAction("Jump");
        slideAction = playerActionMap.FindAction("Slide");
        wallRunAction = playerActionMap.FindAction("WallRun");
        attackAction = playerActionMap.FindAction("Attack");
    }
    private void OnEnable() {
        playerActionMap?.Enable(); 
    }
    private void OnDisable() {
        playerActionMap?.Disable();
    }
    public float velocityY => rb.linearVelocityY;
    private void Update() {
        checkInput();
        if (moveInput != 0) {
            facingDirection = moveInput;
            if (sr != null)
                sr.flipX = facingDirection < 0;
            if (wallCheck != null) {
                Vector3 position = wallCheck.localPosition;
                position.x = Mathf.Abs(position.x) * facingDirection;
                wallCheck.localPosition = position;
            }
        }
        CheckCollision();
        UpdateCoyoteTime();
        bool slideCombo = slideInputHold && moveInput != 0;
        if (slideCombo && !lastSlideCombo)
            SlideButton();
        lastSlideCombo = slideCombo;
        slideTimerCount -= Time.deltaTime;
        slideCoolDownTimer -= Time.deltaTime;
        if (wallRunCooldownTimer > 0f)
            wallRunCooldownTimer -= Time.deltaTime;
        checkForSlideCancle();
    }
    private void checkInput() {
        if (moveAction == null)
        {
            moveInput = 0;
            holdWall = false;
            slideInputHold = false;
            AttackRequested = false;
            lastSlideCombo = false;
            return;
        }
        float horizontal = moveAction.ReadValue<float>();
        moveInput = Mathf.RoundToInt(horizontal);
        slideInputHold = slideAction != null &&
                        slideAction.IsPressed();
        holdWall = wallRunAction != null &&
                wallRunAction.IsPressed();
        if (jumpAction != null &&
            jumpAction.WasPressedThisFrame()) {
            jumpRequested = true;
        }
        AttackRequested = attackAction != null &&
                        attackAction.WasPressedThisFrame();
    }
    private void FixedUpdate() {
        if (rb == null || isWallRunning) return;
        Vector2 velocity = rb.linearVelocity;
        if (velocity.y < -maxFallVelocity) {
            velocity.y = -maxFallVelocity;
            rb.linearVelocity = velocity;
        }
    }
    public void Movement(){
        if (WallFound || wallRunCooldownTimer>0f)
            return;
        Vector2 velocity = rb.linearVelocity;
        if (isSliding){
            velocity.x = facingDirection * slideSpeed;
        }else if (moveInput == 0){
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, deceleration * Time.deltaTime);
            isSuperSprinting = false;
        } else {
            bool reversing = lastMoveInput != 0 && moveInput != lastMoveInput;
            if (reversing && isSuperSprinting) {
                float retainedSpeed = Mathf.Abs(velocity.x);
                velocity.x = moveInput * retainedSpeed;
            }
            else {
                if (lastMoveInput == 0 && Mathf.Abs(velocity.x) < moveSpeed)
                    velocity.x = moveInput * moveSpeed;
                    velocity.x = Mathf.MoveTowards(velocity.x, moveInput * maxSpeed,acceleration * Time.deltaTime);
            }
            if (Mathf.Abs(velocity.x) >= maxSpeed - 0.01f) isSuperSprinting = true;
        }
        rb.linearVelocity = velocity;
        lastMoveInput = moveInput;
    }
    private void UpdateCoyoteTime() {
        if (isWallRunning) {
            lastGroundedTime = float.NegativeInfinity;
            return;
        }
        if (isGrounded && velocityY <= 0f) lastGroundedTime = Time.time;
    }
    public bool HandleJump(){
        if (isSliding || isWallRunning) return false;
        if(Time.time-lastGroundedTime>coyoteTime) return false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        lastGroundedTime = float.NegativeInfinity;
        return true;
    }
    public void ClearJumpRequest() {
        jumpRequested = false;
    }
    public bool TryJump() {
        if(!jumpRequested) return false;
        jumpRequested = false;
        return HandleJump();
    }
    private void CheckCollision(){
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down,GroundCheckDistance, WhatIsGround).collider != null;
        WallFound = wallCheck != null && Physics2D.BoxCast(wallCheck.position, sizeWall, 0f,Vector2.zero, 0f, WhatIsGround).collider != null;
        foundCeilingOnHead = Physics2D.Raycast(transform.position, Vector2.up,DistanceFromHeadToCeiling, WhatIsGround).collider != null;
    }
    private void checkForSlideCancle(){
        if (slideTimerCount < 0f && !foundCeilingOnHead)
            isSliding = false;
    }
    public void SlideButton() {
        if (!isGrounded || isWallRunning) return;
        if (moveInput != 0 && slideCoolDownTimer <= 0f) {
            isSliding = true;
            slideTimerCount = slideTimer;
            slideCoolDownTimer = slideCoolDown;
        }
    }
    public bool canBeginWallRun() {
        return holdWall && isSuperSprinting && WallFound && !isSliding && wallRunCooldownTimer <= 0f;
    }
    public bool UpdateWallRun(){
        if (isWallRunning){
            if (!holdWall){
                isWallRunning = false;
                return false;
            }
            if (moveInput == -wallDirection){
                isWallRunning = false;
                isSuperSprinting = true;
                rb.linearVelocity = new Vector2(moveInput * maxSpeed, yWhenChangeDirection);
                lastMoveInput = moveInput;
                wallRunCooldownTimer = wallRunCooldown;
                return false;
            }
            if (!WallFound){
                isWallRunning = false;
                return false;
            }
        }else {
            if (!holdWall || !isSuperSprinting || !WallFound) return false;
                wallDirection = facingDirection;
                isWallRunning = true;
        }
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(0f, wallRunSpeed);
        return true;
    }
    public void stopWallRun() {
        isWallRunning = false;
        rb.gravityScale = oldGravity;
    }
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y - GroundCheckDistance));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheck.position, sizeWall);
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y + DistanceFromHeadToCeiling));
    }
}
