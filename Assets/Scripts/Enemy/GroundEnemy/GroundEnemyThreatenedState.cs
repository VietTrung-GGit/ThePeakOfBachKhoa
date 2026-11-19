using UnityEngine;
public class GroundEnemyThreatenedState : GroundEnemyState
{
    public GroundEnemyThreatenedState(GroundEnemyStateMachine.EnemyState key) : base(key){}
    public override void OnEnter()
    {
        Debug.Log("Threatened State entered!");
    }
    public override void UpdateState()
    {
        if (!targetEnemy.IsThreatened)
        {
            stateMachine.QueueNextState(GroundEnemyStateMachine.EnemyState.Patrol);
        }
    }

    public override void UpdatePhysics()
    {

    }

    public override void OnExit()
    {
        
    }
}