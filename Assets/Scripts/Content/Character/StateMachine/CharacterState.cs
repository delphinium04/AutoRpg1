using UnityEngine;

namespace Content.Character.StateMachine
{
    public abstract class CharacterState
    {
        protected CharacterController Controller;
        protected CharacterStateMachine StateMachine;
        protected Animator Animator;

        protected CharacterState(CharacterController controller)
        {
            Controller = controller;
            StateMachine = Controller.StateMachine;
            Animator = Controller.Animator;
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