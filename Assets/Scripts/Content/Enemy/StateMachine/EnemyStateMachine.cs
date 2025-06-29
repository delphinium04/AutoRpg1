namespace Content.Enemy.StateMachine
{
    public class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; }

        public void Initialize(EnemyState state)
        {
            CurrentState = state;
            CurrentState.Enter();
        }

        public void ChangeState(EnemyState state)
        {
            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }
}