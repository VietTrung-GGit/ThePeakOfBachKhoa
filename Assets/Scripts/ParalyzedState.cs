using UnityEngine;
public class ParalyzedState : State
{
    protected override void OnEnter()
    {
        Debug.Log("Paralyzed State entered!");
    }

    public override void UpdateState()
    {

        /*Vector2 nextPosition = new Vector2(transform.position.x + speed * Time.deltaTime * (turnRight? 1.0f : -1.0f), transform.position.y);
        if (rb != null)
        {
            rb.MovePosition(nextPosition);
        }*/
        
        if (targetEnemy.IsThreatened)
        {
            stateController.ChangeState(stateController.threatenedState);
        }
        else if (!targetEnemy.IsParalyzed)
        {
            stateController.ChangeState(stateController.patrolState);
        }
    }

    public override void UpdatePhysics()
    {

    }

    public override void OnExit()
    {
        
    }
}