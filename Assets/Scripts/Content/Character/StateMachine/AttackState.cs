// CharacterStateMachine.cs

using Core;
using UnityEngine;

namespace Content.Character.StateMachine
{
    public class AttackState : CharacterState
    {
        // 공격 후 다시 Idle로

        private float _debugTimer;

        private GameObject _target;

        public AttackState(CharacterController controller, GameObject target)
            : base(controller)
        {
            _target = target;
        }

        public override void Enter()
        {
            Animator.SetTrigger(CharacterAnimHash.AttackTrigger);
            Logging.Write("Attack!");
        }


        public override void Update()
        {
            _debugTimer += Time.deltaTime;
            if (_debugTimer > 3)
            {
                StateMachine.ChangeState(new IdleState(Controller));
            }
        }
    }
}