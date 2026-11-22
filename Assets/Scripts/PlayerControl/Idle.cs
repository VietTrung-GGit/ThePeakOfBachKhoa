using UnityEngine;

public class Idle : IState
{
    private Player player;
    private StateController stateController;

    public Idle(Player player, StateController stateController)
    {
        this.player = player;
        this.stateController = stateController;
    }

    public void Enter()
    {
        //Debug.Log("Enter Idle state");
    }

    public void UpdateState()
    {
        if(player.isSliding) {
            stateController.ChangeState(new Slide(player,stateController));
            return;
        } 
        if(player.TryJump()) {
            stateController.ChangeState(new Jump(player,stateController));
            return;
        }
        if(!player.isGrounded && player.velocityY<0f) {
            stateController.ChangeState(new Fall(player,stateController));
            return;
        }
       
        if(player.MoveInput!=0) {
            stateController.ChangeState(new Run(player,stateController));
        }
    }

    public void FixedUpdateState(){
    }

    public void Exit()
    {
        //Debug.Log("Exit idle state");
    }
}