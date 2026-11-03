using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private int moveInput;
    private int lastMoveInput;
    private int facingDirection = 1;
    [SerializeField] private bool isSuperSprinting;
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceDouble;
    [SerializeField] private bool enableDoubleJump = true;
    private bool jumpRequested;
    private bool canDoubleJump;
    [Header("CheckCollision")]
    [SerializeField] private Vector2 sizeWall;
    [SerializeField] private Transform wallCheck;
    public LayerMask WhatIsGround;
    [SerializeField] private float GroundCheckDistance;
    private bool WallFound;
    [SerializeField] private bool isGrounded;
    [Header("SlideInfo")]
    [SerializeField] private float slideTimer;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideCoolDown;
    [SerializeField] private float DistanceFromHeadToCeiling;
    [HideInInspector] public float slideCoolDownTimer;
    private float slideTimerCount;
    [SerializeField] private bool isSliding;
    private bool foundCeilingOnHead;
    private bool lastSlideCombo;

    [Header("Wall Running")]
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float yWhenChangeDirection;
    [SerializeField] private bool isWallRunning;
    private bool holdWall;
    private int wallDirection;
    private float oldGravity;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        oldGravity = rb.gravityScale;
    }
    private void Update() {
        checkInput();
        CheckCollision();
        if (moveInput != 0) {
            facingDirection = moveInput;
            if (sr != null)
                sr.flipX = facingDirection < 0;
            if (wallCheck != null){
                Vector3 p = wallCheck.localPosition;
                p.x = Mathf.Abs(p.x) * facingDirection;
                wallCheck.localPosition = p;
            }
        }
         Keyboard k = Keyboard.current;
        bool slideCombo = k.sKey.isPressed && moveInput != 0;
        if (slideCombo && !lastSlideCombo) SlideButton();
        lastSlideCombo = slideCombo;
        slideTimerCount -= Time.deltaTime;
        slideCoolDownTimer -= Time.deltaTime;

        if (isGrounded) canDoubleJump = enableDoubleJump;
        checkForSlideCancle();
        bool jumped = jumpRequested && HandleJump();
        jumpRequested = false;
        rb.gravityScale = oldGravity;
        if (jumped || !UpdateWallRun())
            Movement();
    }
    private void checkInput() {
        Keyboard k = Keyboard.current;
        if (k == null) {
            moveInput = 0;
            holdWall = false;
            lastSlideCombo = false;
            return;
        }
        moveInput = (k.dKey.isPressed ? 1 : 0) - (k.aKey.isPressed ? 1 : 0);
        holdWall = k.wKey.isPressed;
        if (k.spaceKey.wasPressedThisFrame)
            jumpRequested = true;
    }
    private void Movement(){
        if (WallFound)
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
                velocity.x = moveInput * maxSpeed;
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
    private bool HandleJump(){
        if (isSliding) return false;
        float force;
        if (isGrounded){
            force = jumpForce;
        }else if (canDoubleJump){
            force = jumpForceDouble;
            canDoubleJump = false;
        } else {
            return false;
        }
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        isGrounded = false;
        isWallRunning = false;
        return true;
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
        if (moveInput != 0 && slideCoolDownTimer <= 0f){
            isSliding = true;
            slideTimerCount = slideTimer;
            slideCoolDownTimer = slideCoolDown;
        }
    }
    private bool UpdateWallRun(){
        if (isWallRunning){
            if (!holdWall){
                isWallRunning = false;
                return false;
            }
            if (moveInput == -wallDirection){
                isWallRunning = false;
                isSuperSprinting = true;
                rb.linearVelocity = new Vector2(
                    moveInput * maxSpeed, yWhenChangeDirection);
                lastMoveInput = moveInput;
                return true;
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
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y - GroundCheckDistance));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheck.position, sizeWall);
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y + DistanceFromHeadToCeiling));
    }
}
