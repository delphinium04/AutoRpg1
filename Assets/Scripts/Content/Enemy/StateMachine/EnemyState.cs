using Content.Character;
using UnityEngine;

namespace Content.Enemy.StateMachine
{
    public abstract class EnemyState
    {
        protected EnemyController Controller;
        protected EnemyStateMachine StateMachine;
        protected Animator Animator;

        protected EnemyState(EnemyController controller)
        {
            Controller = controller;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
        }
    }
}