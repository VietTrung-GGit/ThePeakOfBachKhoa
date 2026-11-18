using UnityEngine;
public class Fall : IState
{
    private Player player;
    private StateController stateController;
    public Fall(Player player, StateController stateController) {
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
        if (!player.isGrounded || player.velocityY > 0f)
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