
using Content.Character.Common;
using UnityEngine;

namespace Content.Enemy.StateMachine
{
    public class AttackState : EnemyState
    {
        // 공격 후 다시 Idle로

        private float _debugTimer;

        private GameObject _target;

        public AttackState(EnemyController controller, GameObject target)
            : base(controller)
        {
            _target = target;
        }

        public override void Enter()
        {
            Animator.SetTrigger(CharacterAnimHash.AttackTrigger);
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