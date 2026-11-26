using UnityEngine;
public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerStateMachine.PlayerEnumState key) : base(key){}

    private float currentCoyoteTime;
    private bool isCoyoteTimeRunning = false;

    public override void OnEnter()
    {
        currentCoyoteTime = player.coyoteTime;
        Debug.Log("Player Fall State entered");
    }

    public override void UpdateState()
    {
        HandleCoyoteJump();

        if (player.IsHurt)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Hurt);
        }
        else if (player.IsGrounded)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Idle);
        }
        else if (player.CanRestartWallRun)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.WallRun);
        }
        else if (player.CanCoyoteJump && player.IsReadyToJump && isCoyoteTimeRunning)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Jump);
        }
    }

    private void HandleCoyoteJump()
    {
        if (player.CanCoyoteJump && !isCoyoteTimeRunning)
        {
            isCoyoteTimeRunning = true;
        }

        if (isCoyoteTimeRunning)
        {
            currentCoyoteTime -= Time.deltaTime;
            if (currentCoyoteTime <= 0.0f)
            {
                isCoyoteTimeRunning = false;
                currentCoyoteTime = player.coyoteTime;
            }
        }
    }

    public override void UpdatePhysics()
    {
        player.PlayerRigidbody.linearVelocityX = Mathf.MoveTowards(player.PlayerRigidbody.linearVelocityX, player.MoveValue * player.maxGroundSpeed, player.groundAcceleration * Time.deltaTime);
        //player.PlayerRigidbody.linearVelocityY = Mathf.Clamp(player.PlayerRigidbody.linearVelocityY, - player.maxFallVelocity, Mathf.Infinity);
        player.PlayerRigidbody.linearVelocityY = Mathf.MoveTowards(player.PlayerRigidbody.linearVelocityY, - player.maxFallVelocity, player.fallAcceleration * Time.deltaTime);
    }

    public override void OnExit()
    {
        isCoyoteTimeRunning = false;
        currentCoyoteTime = player.coyoteTime;
    }
}