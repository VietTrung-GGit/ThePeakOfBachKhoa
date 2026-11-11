using System.Collections;
using UnityEngine;

public class FallState : State
{
    protected override void OnEnter()
    {
        Debug.Log("Fall State entered!");
    }

    public override void UpdateState()
    {
        if (targetEnemy.IsGrounded())
        {
            //targetEnemy.Body.linearVelocityY = 0.0f;
            
            stateController.ChangeState(stateController.patrolState);
        }
    }


    public override void UpdatePhysics()
    {
        //targetEnemy.Body.linearVelocityY -= targetEnemy.gravityAcceleration * Time.deltaTime;
    }

    public override void OnExit()
    {

    }
}