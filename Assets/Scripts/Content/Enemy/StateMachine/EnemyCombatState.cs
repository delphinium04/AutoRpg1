using Content.Character;
using Core;
using UnityEngine;
using CharacterController = Content.Character.CharacterController;

namespace Content.Enemy.StateMachine
{
    public class EnemyCombatState : EnemyState
    {
        private float _attackTimer;
        private float _moveSpeed = 0.5f;
        private float _debugAttackInterval = 1.5f;
        private float _debugAttackReach = 1f;

        private Vector3 _positionDelta;

        public EnemyCombatState(EnemyController controller)
            : base(controller)
        {
        }

        public override void Enter()
        {
            Animator.SetBool(EnemyAnimHash.MoveBool, true);
            if (!Controller.TargetCharacter)
                StateMachine.ChangeState(new EnemyIdleState(Controller));
        }

        public override void Update()
        {
            if (!Controller.TargetCharacter)
            {
                StateMachine.ChangeState(new EnemyIdleState(Controller));
                return;
            }
            
            _attackTimer += Time.deltaTime;

            _positionDelta = Controller.TargetCharacter.transform.position - Controller.transform.position;
            float distance = _positionDelta.magnitude;

            if (distance > _debugAttackReach && _attackTimer > _debugAttackInterval)
            {
                _attackTimer = 0;
                Attack();
            }
            else
            {
                Move();
            }
        }

        private void Move()
        {
            var tan = Mathf.Atan2(_positionDelta.z, _positionDelta.x);
            var rotation = Quaternion.Euler(0, tan, 0);
            Controller.transform.SetPositionAndRotation(
                Controller.transform.position + _positionDelta * (Time.deltaTime * _moveSpeed), rotation);
        }

        private void Attack()
        {
            Animator.SetTrigger(EnemyAnimHash.AttackTrigger);
            Controller.TargetCharacter.TakeDamage(10);
        }
    }
}