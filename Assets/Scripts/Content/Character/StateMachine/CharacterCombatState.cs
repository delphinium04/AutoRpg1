using Core;
using UnityEngine;

namespace Content.Character.StateMachine
{
    public class CharacterCombatState : CharacterState
    {
        private Vector3 _positionDelta;

        private float _moveSpeed = 1.2f;
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
                // 사거리 내에 있지만 쿨타임
                if (_attackTimer < _attackInterval) return;
                _attackTimer = 0;
                Attack();

                return;
            }

            MoveToTarget();
        }

        private void MoveToTarget()
        {
            Quaternion rotation = Quaternion.LookRotation(_positionDelta);

            Controller.transform.SetPositionAndRotation(
                Controller.transform.position + _positionDelta * (Time.deltaTime * _moveSpeed), rotation);
        }

        private void Attack()
        {
            Logging.Write("Attacked");
            
            Animator.SetTrigger(CharacterAnimHash.AttackTrigger);
            Controller.TargetEnemy.TakeDamage(Controller.Data.Attack);
        }
    }
}