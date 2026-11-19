using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, State<EState>> stateDictionary = new Dictionary<EState, State<EState>>();
    protected State<EState> currentState;
    private EState queuedState = default;

    private void Update()
    {
        if (currentState != null && queuedState.Equals(currentState.StateKey))
        {
            currentState?.UpdateState();
        }
        else
        {
            ChangeState(queuedState);
        }
    }

    private void FixedUpdate()
    {
        currentState?.UpdatePhysics();
    }

    public void QueueNextState(EState nextState)
    {
        queuedState = nextState;
    }

    protected void ChangeState(EState newState)
    {
        currentState?.OnExit();
        currentState = stateDictionary[newState];
        SetUpState();
        currentState.OnEnter();
    }

    protected abstract void SetUpState();
}