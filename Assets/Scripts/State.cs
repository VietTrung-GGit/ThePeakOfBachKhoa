using System;

public abstract class State<EState> where EState : Enum
{
    public EState StateKey {get; private set;}
    /*public void OnStateEnter(StateMachine<EState> controller, GroundEnemy enemy)
    {
        stateMachine = controller;
        targetEnemy = enemy;
        OnEnter();
    }*/

    public State(EState key)
    {
        StateKey = key;
    }
    public virtual void OnEnter(){}
    public virtual void UpdateState(){}
    public virtual void UpdatePhysics(){}
    public virtual void OnExit(){}
}