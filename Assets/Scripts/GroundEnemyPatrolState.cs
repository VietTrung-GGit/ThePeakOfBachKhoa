using UnityEngine;

public class GroundEnemyPatrolState : GroundEnemyState
{
    public GroundEnemyPatrolState(GroundEnemyStateMachine.EnemyState key) : base(key){}
    public override void OnEnter()
    {
        Debug.Log("Patrol State entered!");
    }
    public override void UpdateState()
    {
        if (targetEnemy.IsThreatened)
        {
            targetEnemy.Body.linearVelocityX = 0.0f;
            stateMachine.QueueNextState(GroundEnemyStateMachine.EnemyState.Threatened);
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