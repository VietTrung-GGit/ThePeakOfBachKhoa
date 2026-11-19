using UnityEngine;
public class GroundEnemyParalyzedState : GroundEnemyState
{
    public GroundEnemyParalyzedState(GroundEnemyStateMachine.EnemyState key) : base(key){}
    public override void OnEnter()
    {
        Debug.Log("Paralyzed State entered!");
    }

    public override void UpdateState()
    {
        if (targetEnemy.IsThreatened)
        {
            stateMachine.QueueNextState(GroundEnemyStateMachine.EnemyState.Threatened);
        }
        else if (!targetEnemy.IsParalyzed)
        {
            stateMachine.QueueNextState(GroundEnemyStateMachine.EnemyState.Paralyzed);
        }
    }

    public override void UpdatePhysics()
    {

    }

    public override void OnExit()
    {
        
    }
}