using UnityEngine;
public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerStateMachine.PlayerEnumState key) : base(key){}



    public override void OnEnter()
    {
        Debug.Log("Player Jump State entered");
        player.CanCoyoteJump = false;
        float jumpImpulse = player.PlayerRigidbody.mass * (player.jumpForce - player.PlayerRigidbody.linearVelocityY);
        player.PlayerRigidbody.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
    }

    public override void UpdateState()
    {
        if (player.IsHurt)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Hurt);
        }
        else if (player.IsFalling)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Fall);
        }
        else if (player.IsGrounded)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Idle);
        }
        else if (player.CanRestartWallRun)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.WallRun);
        }
    }

    public override void UpdatePhysics()
    {
        player.PlayerRigidbody.linearVelocityX = Mathf.MoveTowards(player.PlayerRigidbody.linearVelocityX, player.MoveValue * player.maxGroundSpeed, player.groundAcceleration * Time.deltaTime);
    }

    public override void OnExit()
    {
        
    }
}