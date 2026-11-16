public abstract class State
{
    protected StateController stateController;
    protected GroundEnemy targetEnemy;
    public void OnStateEnter(StateController controller, GroundEnemy enemy)
    {
        stateController = controller;
        targetEnemy = enemy;
        OnEnter();
    }
    protected virtual void OnEnter(){}
    public virtual void UpdateState(){}
    public virtual void UpdatePhysics(){}
    public virtual void OnExit(){}
}