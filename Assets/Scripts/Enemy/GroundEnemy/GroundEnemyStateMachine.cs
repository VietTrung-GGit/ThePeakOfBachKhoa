using UnityEngine;

[RequireComponent(typeof(GroundEnemy))]
public class GroundEnemyStateMachine : StateMachine<GroundEnemyStateMachine.EnemyState>
{
    public enum EnemyState
    {
        Patrol,
        Threatened,
        Paralyzed
    }
    private GroundEnemy targetEnemy;
    public GroundEnemyPatrolState patrolState = new GroundEnemyPatrolState(EnemyState.Patrol);
    public GroundEnemyThreatenedState threatenedState = new GroundEnemyThreatenedState(EnemyState.Threatened);
    public GroundEnemyParalyzedState paralyzedState = new GroundEnemyParalyzedState(EnemyState.Paralyzed);
    private void Awake()
    {
        targetEnemy = GetComponent<GroundEnemy>();
        UpdateStateDictionary();
    }

    private void UpdateStateDictionary()
    {
        stateDictionary.Add(EnemyState.Patrol, patrolState);
        stateDictionary.Add(EnemyState.Threatened, threatenedState);
        stateDictionary.Add(EnemyState.Paralyzed, paralyzedState);
    }

    private void Start()
    {
        QueueNextState(EnemyState.Patrol);
    }

    protected override void SetUpState()
    {
        if (currentState is GroundEnemyState state)
        {
            state.OnStateEnter(this, targetEnemy);
        }
    }
}