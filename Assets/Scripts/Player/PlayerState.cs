public abstract class PlayerState : State<PlayerStateMachine.PlayerEnumState>
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    public PlayerState(PlayerStateMachine.PlayerEnumState key) : base(key){}
    public void OnStateEnter(PlayerStateMachine controller, Player player)
    {
        stateMachine = controller;
        this.player = player;
    }
}