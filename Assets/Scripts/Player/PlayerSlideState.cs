using UnityEngine;
public class PlayerSlideState : PlayerState
{
    public PlayerSlideState(PlayerStateMachine.PlayerEnumState key) : base(key){}

    public override void OnEnter()
    {
        Debug.Log("Player Slide State entered");
        player.UpdatePlayerCollider(true);
    }

    public override void UpdateState()
    {
        if (player.IsHurt)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Hurt);
        }
        else if (!player.IsSliding)
        {
            if (player.IsFalling)
            {
                stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Fall);
            }
            else if (player.MoveValue != 0.0f)
            {
                stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Run);
            }
            else if (player.IsReadyToJump)
            {
                stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Jump);
            }
            else
            {
                stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Idle);
            }
        }
    }

    public override void UpdatePhysics()
    {
        player.PlayerRigidbody.linearVelocityX = Mathf.MoveTowards(player.PlayerRigidbody.linearVelocityX, player.MoveValue * player.maxGroundSpeed, player.groundAcceleration * Time.deltaTime);
    }

    public override void OnExit()
    {
        player.UpdatePlayerCollider(false);
    }
}