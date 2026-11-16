using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class StateController : MonoBehaviour
{
    private GroundEnemy targetEnemy;
    //private IState currentState;
    private State currentState;

    public PatrolState patrolState = new PatrolState();
    public ThreatenedState threatenedState = new ThreatenedState();
    public ParalyzedState paralyzedState = new ParalyzedState();

    private void Awake()
    {
        targetEnemy = GetComponent<GroundEnemy>();
    }
    private void Start()
    {
        ChangeState(patrolState);
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState?.UpdatePhysics();
    }

    public void ChangeState(State newState)
    {
        currentState?.OnExit();
        currentState = newState;
        newState.OnStateEnter(this, targetEnemy);
    }
}