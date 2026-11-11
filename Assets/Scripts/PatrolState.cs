using System.Collections;
using UnityEngine;

public class PatrolState : State
{
    protected override void OnEnter()
    {
        Debug.Log("Patrol State entered!");
    }

    public override void UpdateState()
    {

        /*Vector2 nextPosition = new Vector2(transform.position.x + speed * Time.deltaTime * (turnRight? 1.0f : -1.0f), transform.position.y);
        if (rb != null)
        {
            rb.MovePosition(nextPosition);
        }*/

        /*if (!targetEnemy.IsGrounded())
        {
            stateController.ChangeState(stateController.fallState);
        }*/
        if (targetEnemy.IsThreatened)
        {
            targetEnemy.Body.linearVelocityX = 0.0f;
            stateController.ChangeState(stateController.threatenedState);
        }
    }

    public override void UpdatePhysics()
    {
        targetEnemy.Body.linearVelocityX = targetEnemy.Direction * targetEnemy.speed;
    }

    public override void OnExit()
    {
        
    }
}