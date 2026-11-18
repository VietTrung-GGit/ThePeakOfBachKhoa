using UnityEngine;
[DefaultExecutionOrder(100)]
public class StateController : MonoBehaviour
{
    private IState currentState;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start() {
        ChangeState(new Idle(player,this));
    }
    private void Update() {
        if (currentState == null) return;
        bool wasWallRunning = currentState is WallRun;
        currentState.UpdateState();
        if (!wasWallRunning && !(currentState is WallRun))
            player.Movement();
        player.ClearJumpRequest();
    }
    private void FixedUpdate() {
        if (currentState != null)
            currentState.FixedUpdateState();
    }
    public void ChangeState(IState newState) {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (currentState != null)
            currentState.Enter();
    }
}