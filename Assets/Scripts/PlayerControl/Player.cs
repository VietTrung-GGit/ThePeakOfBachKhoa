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
    [SerializeField, Min(0f)] private float jumpBufferTime = 0.12f;
    private float jumpRequestTime = float.NegativeInfinity;
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

    [Header("Slide Collider")]
    [SerializeField] private float slideHeightMultiplier = 0.5f;
    private BoxCollider2D col;
    private Vector2 normalSize, normalOffset;
    private bool slideColliderApplied;
    
    [Header("Camera things")]
    [SerializeField] private CameraFollow cameraFollow;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    if (col != null) { normalSize = col.size; normalOffset = col.offset; }
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
            if (moveInput != facingDirection)
            {
                facingDirection = moveInput;
                cameraFollow?.CallTurn();
            }
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
        if (jumpRequested && Time.time - jumpRequestTime > jumpBufferTime)
            jumpRequested = false;
        bool slideCombo = slideInputHold && moveInput != 0;
        if (slideCombo && !lastSlideCombo)
            SlideButton();
        lastSlideCombo = slideCombo;
        slideTimerCount -= Time.deltaTime;
        slideCoolDownTimer -= Time.deltaTime;
        if (wallRunCooldownTimer > 0f)
            wallRunCooldownTimer -= Time.deltaTime;
        checkForSlideCancle();
        UpdateSlideCollider();
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
            jumpRequestTime = Time.time;
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
        if (isGrounded && velocityY <= 0.1f) lastGroundedTime = Time.time;
    }
    #region Jump
    public bool HandleJump(){
        if (isSliding || isWallRunning) {
            if (isSliding)
            {
                Debug.Log("Player is sliding");
            }
            else
            {
                Debug.Log("Player is wall running");
            }
            return false;
        }
        
        if(Time.time-lastGroundedTime>coyoteTime) {
            Debug.Log("Not in coyote time" + lastGroundedTime);
            return false;
        }
        Debug.Log("Jump applied");
        float jumpImpulse = rb.mass * (jumpForce - rb.linearVelocityY);
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        isGrounded = false;
        lastGroundedTime = float.NegativeInfinity;
        return true;
    }
    public bool TryJump() {
        if (!jumpRequested || Time.time - jumpRequestTime > jumpBufferTime || !HandleJump()) return false;
        jumpRequested = false;
        return true;
    }

    #endregion
    private void CheckCollision(){
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down,GroundCheckDistance, WhatIsGround).collider != null;
        WallFound = wallCheck != null && Physics2D.BoxCast(wallCheck.position, sizeWall, 0f,Vector2.zero, 0f, WhatIsGround).collider != null;
        foundCeilingOnHead = Physics2D.Raycast(transform.position, Vector2.up,DistanceFromHeadToCeiling, WhatIsGround).collider != null;
    }
    #region Sliding
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
    private void UpdateSlideCollider() {
    if (col == null || isSliding == slideColliderApplied) return;
    slideColliderApplied = isSliding;
    if (isSliding) {
        float newHeight = normalSize.y * slideHeightMultiplier;
        col.size = new Vector2(normalSize.x, newHeight);
        // lower the offset so the feet stay on the ground
        col.offset = new Vector2(normalOffset.x, normalOffset.y - (normalSize.y - newHeight) / 2f);
    } else {
        col.size = normalSize;
        col.offset = normalOffset;
    }
    }
    #endregion

    #region WallRun
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
    #endregion
    
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y - GroundCheckDistance));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheck.position, sizeWall);
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y + DistanceFromHeadToCeiling));
    }
    public bool isFacingRight()
    {
        return facingDirection > 0;
    }
    public float getmaxSpeed()
    {
        return maxSpeed;
    }
    public float getXvelocity()
    {
        return rb.linearVelocityX;
    }
    public float getYVelocity()
    {
        return rb.linearVelocity.y;
    }
}
