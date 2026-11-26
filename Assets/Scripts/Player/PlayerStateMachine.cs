using UnityEngine;
[RequireComponent(typeof(Player))]
public class PlayerStateMachine : StateMachine<PlayerStateMachine.PlayerEnumState>
{
    public enum PlayerEnumState
    {
        Idle,
        Fall,
        Run,
        WallRun,
        Slide,
        Jump,
        Hurt
    }

    private Player player;
    public PlayerIdleState playerIdleState = new PlayerIdleState(PlayerEnumState.Idle);
    public PlayerFallState playerFallState = new PlayerFallState(PlayerEnumState.Fall);
    public PlayerRunState playerRunState = new PlayerRunState(PlayerEnumState.Run);
    public PlayerWallRunState playerWallRunState = new PlayerWallRunState(PlayerEnumState.WallRun);
    public PlayerSlideState playerSlideState = new PlayerSlideState(PlayerEnumState.Slide);
    public PlayerJumpState playerJumpState = new PlayerJumpState(PlayerEnumState.Jump);
    public PlayerHurtState playerHurtState = new PlayerHurtState(PlayerEnumState.Hurt);
    private void Awake()
    {
        player = GetComponent<Player>();
        UpdateStateDictionary();
    }

    private void UpdateStateDictionary()
    {
        stateDictionary.Add(PlayerEnumState.Run, playerRunState);
        stateDictionary.Add(PlayerEnumState.WallRun, playerWallRunState);
        stateDictionary.Add(PlayerEnumState.Fall, playerFallState);
        stateDictionary.Add(PlayerEnumState.Slide, playerSlideState);
        stateDictionary.Add(PlayerEnumState.Idle, playerIdleState);
        stateDictionary.Add(PlayerEnumState.Jump, playerJumpState);
        stateDictionary.Add(PlayerEnumState.Hurt, playerHurtState);
    }

    protected override void SetUpState()
    {
        if (currentState is PlayerState state)
        {
            state.OnStateEnter(this, player);
        }
    }
}