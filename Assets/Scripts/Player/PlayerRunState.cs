using UnityEngine;
public class PlayerRunState : PlayerState
{
    private bool isSuperSprintGraceTimeRunning = false;
    private float currentSuperSprintGraceTime;

    public PlayerRunState(PlayerStateMachine.PlayerEnumState key) : base(key){}

    public override void OnEnter()
    {
        currentSuperSprintGraceTime = player.superSprintGraceTime;
        Debug.Log("Player Run State entered");
    }

    public override void UpdateState()
    {
        HandleGraceSuperSprint();
        HandleReverseSuperSprint();

        if (player.IsHurt)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Hurt);
        }
        else if (player.IsFalling)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Fall);
        }
        if (player.MoveValue == 0.0f && !player.IsSuperSprinting)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Idle);
        }
        else if (player.CanStartWallRun || player.CanRestartWallRun)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.WallRun);
        }
        else if (player.IsSliding)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Slide);
        }
        else if (player.IsReadyToJump)
        {
            stateMachine.QueueNextState(PlayerStateMachine.PlayerEnumState.Jump);
        }
    }

    private void HandleGraceSuperSprint()
    {
        if (player.IsSuperSprinting && player.PlayerRigidbody.linearVelocityX == 0.0f)
        {
            isSuperSprintGraceTimeRunning = true;
        }

        if (isSuperSprintGraceTimeRunning)
        {
            currentSuperSprintGraceTime -= Time.deltaTime;
            if (currentSuperSprintGraceTime <= 0.0f)
            {
                player.IsSuperSprinting = false;
                isSuperSprintGraceTimeRunning = false;
                currentSuperSprintGraceTime = player.superSprintGraceTime;
            }
        }
    }
    private void HandleReverseSuperSprint()
    {

        if (player.IsReversingDirection && player.IsSuperSprinting)
        {
            Debug.Log("Speed is retained!");
            player.PlayerRigidbody.linearVelocityX = player.maxGroundSpeed * player.FacingDirection;
        }
    }

    public override void UpdatePhysics()
    {
        player.PlayerRigidbody.linearVelocityX = Mathf.MoveTowards(player.PlayerRigidbody.linearVelocityX, player.MoveValue * player.maxGroundSpeed, player.groundAcceleration * Time.deltaTime);
    }

    public override void OnExit()
    {
        player.IsSuperSprinting = false;
        isSuperSprintGraceTimeRunning = false;
        currentSuperSprintGraceTime = player.superSprintGraceTime;
    }
}