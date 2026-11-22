using UnityEngine;

public class Jump : IState
{
    private Player player;
    private StateController stateController;

    public Jump(Player player, StateController stateController) {
        this.player = player;
        this.stateController = stateController;
    }

    public void Enter()
    {
        //Debug.Log("Enter Jump State");
    }

    public void UpdateState() {
        if(player.canBeginWallRun()) {
            stateController.ChangeState(new WallRun(player,stateController));
            return;
        }
        if(player.velocityY<=0f) {
            stateController.ChangeState(new Fall(player,stateController));
        }
    }

    public void FixedUpdateState() {
    }

    public void Exit() {
        //Debug.Log("Exit Jump State");
    }
}
