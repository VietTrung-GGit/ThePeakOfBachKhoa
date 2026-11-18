using UnityEngine;

public class WallRun : IState
{
    private Player player;
    private StateController stateController;

    public WallRun(Player player, StateController stateController) {
        this.player = player;
        this.stateController = stateController;
    }

    public void Enter() {
       player.UpdateWallRun();
    }
    public void UpdateState() {
        if (player.UpdateWallRun()) return;
        if (player.velocityY > 0f) {
            stateController.ChangeState(new Jump(player, stateController));
            return;
        }
        if (!player.isGrounded) {
            stateController.ChangeState(new Fall(player, stateController));
            return;
        }
        if (player.MoveInput != 0) stateController.ChangeState(new Run(player, stateController));
        else stateController.ChangeState(new Idle(player, stateController));
    }
    public void FixedUpdateState() {
    }

    public void Exit() {
       player.stopWallRun();
    }
}