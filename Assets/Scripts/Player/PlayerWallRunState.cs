using UnityEngine;
public class PlayerWallRunState : PlayerState
{
    public PlayerWallRunState(PlayerStateMachine.PlayerEnumState key) : base(key){}

    public override void OnEnter()
    {
        Debug.Log("Player WallRun State entered");
        player.CanWallRunContinue = true;
        player.PlayerRigidbody.gravityScale = player.wallRunGravityScale;
    }

    public override void UpdateState()
    {
        if (player.IsHurt)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Hurt);
        }
        else if (!player.IsReadyToWallRun)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Fall);
        }
    }

    public override void UpdatePhysics()
    {
        player.PlayerRigidbody.linearVelocity = new Vector2(0.0f, player.maxWallRunSpeed);
    }

    public override void OnExit()
    {
        player.PlayerRigidbody.gravityScale = player.groundGravityScale;
    }
}