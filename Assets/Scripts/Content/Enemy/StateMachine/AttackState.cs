using Content.Character;
using Core;
using UnityEngine;
using CharacterController = Content.Character.CharacterController;

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
            if (_target?.TryGetComponent<CharacterController>(out var damageable) == true)
            {
                Logging.Write("Attack!");
                damageable.TakeDamage(30);
            }
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