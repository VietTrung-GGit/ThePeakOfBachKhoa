public abstract class GroundEnemyState : State<GroundEnemyStateMachine.EnemyState>
{
    protected GroundEnemyStateMachine stateMachine;
    protected GroundEnemy targetEnemy;

    public GroundEnemyState(GroundEnemyStateMachine.EnemyState key) : base(key){}
    public void OnStateEnter(GroundEnemyStateMachine controller, GroundEnemy enemy)
    {
        stateMachine = controller;
        targetEnemy = enemy;
    }
}