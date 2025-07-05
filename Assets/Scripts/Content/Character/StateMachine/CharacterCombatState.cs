using Core;
using UnityEngine;

namespace Content.Character.StateMachine
{
    public class CharacterCombatState : CharacterState
    {
        private Vector3 _positionDelta;

        private float _moveSpeed = 2;
        private float _attackTimer = 0;
        private float _attackInterval = 1;

        public CharacterCombatState(CharacterController controller) : base(controller)
        {
        }

        public override void Enter()
        {
            if (!Controller.TargetEnemy)
            {
                StateMachine.ChangeState(new CharacterIdleState(Controller));
            }

            _attackInterval = 1 / Mathf.Min(Controller.Data.AttackSpeed, 0.01f);
            Animator.SetBool(CharacterAnimHash.MoveBool, true);
        }

        public override void Exit()
        {
            Animator.SetBool(CharacterAnimHash.MoveBool, false);
        }

        public override void Update()
        {
            if (Controller.TargetEnemy?.IsDead ?? true)
            {
                StateMachine.ChangeState(new CharacterIdleState(Controller));
                return;
            }

            _attackTimer += Time.deltaTime;

            // 공격 거리 체크 및 이동
            _positionDelta = Controller.TargetEnemy.transform.position - Controller.transform.position;
            if (_positionDelta.magnitude < Controller.Data.AttackReach)
            {
                if (_attackTimer >= _attackInterval)
                {
                    _attackTimer = 0;
                    Attack();
                }

                return;
            }

            MoveToTarget();
        }

        private void MoveToTarget()
        {
            float tan = Mathf.Atan2(_positionDelta.z, _positionDelta.x);
            Quaternion rotation = Quaternion.Euler(0, Mathf.Rad2Deg * tan, 0);

            Controller.transform.SetPositionAndRotation(
                Controller.transform.position + _positionDelta * (Time.deltaTime * _moveSpeed), rotation);
        }

        private void Attack()
        {
            Animator.SetTrigger(CharacterAnimHash.AttackTrigger);
            Controller.TargetEnemy.TakeDamage(Controller.Data.Attack);
        }
    }
}