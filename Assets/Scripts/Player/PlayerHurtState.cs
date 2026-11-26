using UnityEngine;
public class PlayerHurtState : PlayerState
{
    public PlayerHurtState(PlayerStateMachine.PlayerEnumState key) : base(key){}

    public override void OnEnter()
    {
        float knockbackImpulse = player.PlayerRigidbody.mass * (player.knockbackForce - player.PlayerRigidbody.linearVelocityY);
        player.PlayerRigidbody.AddForce(new Vector2(- knockbackImpulse * player.FacingDirection, knockbackImpulse), ForceMode2D.Impulse);
        Debug.Log("Player Hurt State entered");
    }

    public override void UpdateState()
    {
        if (player.IsFalling)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Fall);
        }
        else if (player.IsGrounded)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Idle);
        }
    }

    public override void UpdatePhysics()
    {
        
    }
    public override void OnExit()
    {
        player.IsHurt = false;
    }
}