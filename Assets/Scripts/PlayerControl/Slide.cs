using UnityEngine;

public class Slide : IState
{
    private OldPlayer player;
    private StateController stateController;

    public Slide(OldPlayer player, StateController stateController) {
        this.player = player;
        this.stateController = stateController;
    }

    public void Enter() {
        Debug.Log("Enter Slide State");
    }

    public void UpdateState() {
       if (player.isSliding) return;
        if (!player.isGrounded){
            stateController.ChangeState(new Fall(player, stateController));
            return;
        }
        if (player.MoveInput != 0) stateController.ChangeState(new Run(player, stateController));
        else stateController.ChangeState(new Idle(player, stateController));
    }
    public void FixedUpdateState() {
    }

    public void Exit() {
        Debug.Log("Exit Slide State");
    }
}