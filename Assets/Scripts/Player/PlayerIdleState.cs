using UnityEngine;
public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine.PlayerEnumState key) : base(key){}

    public override void OnEnter()
    {
        Debug.Log("Player Idle State entered");
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
        else if (player.MoveValue != 0.0f)
        {
            if (player.IsSliding)
            {
                stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Slide);
            }
            else
            {
                stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Run);
            }
        }
        else if (player.IsReadyToJump)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Jump);
        }
    }

    public override void UpdatePhysics()
    {
        player.PlayerRigidbody.linearVelocityX = Mathf.MoveTowards(player.PlayerRigidbody.linearVelocityX, 0.0f, player.groundAcceleration * Time.deltaTime);
    }

    public override void OnExit()
    {
        
    }
}