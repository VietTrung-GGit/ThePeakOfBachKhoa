using UnityEngine;
public class Fall : IState
{
    private OldPlayer player;
    private StateController stateController;
    public Fall(OldPlayer player, StateController stateController) {
        this.player = player;
        this.stateController = stateController;
    }
    public void Enter() {
        Debug.Log("Enter Fall State");
    }
    public void UpdateState() {
        if (player.canBeginWallRun()) {
            stateController.ChangeState(new WallRun(player, stateController));
            return;
        }
        if(player.TryJump()) {
            stateController.ChangeState(new Jump(player,stateController));
            return;
        }
        if (!player.isGrounded || player.velocityY > 0.1f)
            return;
        if (player.isSliding) {
            stateController.ChangeState(new Slide(player, stateController));
            return;
        }
        if (player.MoveInput != 0) stateController.ChangeState(new Run(player, stateController));
        else stateController.ChangeState(new Idle(player, stateController));
    }
    public void FixedUpdateState() {
    }

    public void Exit()
    {
        Debug.Log("Exit Fall State");
    }
}