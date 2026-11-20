using UnityEngine;
public interface IState
{
    void Enter();
    void UpdateState();
    void FixedUpdateState();
    void Exit();
}