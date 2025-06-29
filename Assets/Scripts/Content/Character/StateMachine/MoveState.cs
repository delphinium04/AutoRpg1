using Content.Character.Common;
using Content.Enemy;
using UnityEngine;

namespace Content.Character.StateMachine
{
    public class MoveState : CharacterState
    {
        /*
         * (수동) 이동을 멈출 경우 -> Idle
         * (자동) 적이 공격 거리 내 들어온 경우 -> Attack
         */

        private float _attackReach = 2f;
        private Vector2 _moveInput;
        private Transform _characterTransform;
        private EnemyController _enemy;
        private Transform _enemyTransform;

        public MoveState(CharacterController controller, GameObject target = null)
            : base(controller)
        {
            _moveInput = Vector3.zero;
            _characterTransform = controller.transform;

            if (target && target.TryGetComponent<EnemyController>(out var component))
            {
                _enemy = component;
                _enemyTransform = target.transform;
            }
        }

        public override void Enter()
        {
            Animator.SetBool(CharacterAnimHash.MoveBool, true);
        }

        public override void Update()
        {
            if (Controller._isAutoMode)
            {
                AutoMove();
                return;
            }

            // 수동 이동
            _moveInput = InputHandler.MoveInput;
            if (_moveInput.sqrMagnitude < 0.1f)
            {
                StateMachine.ChangeState(new IdleState(Controller));
                return;
            }

            ManualMove();
        }

        public override void Exit()
        {
            Animator.SetBool(CharacterAnimHash.MoveBool, false);
        }

        private void AutoMove()
        {
            _characterTransform.LookAt(_enemyTransform);
            _characterTransform.Translate(Vector3.forward * (Controller._moveSpeed * Time.deltaTime));

            if (Vector3.Distance(_characterTransform.position, _enemyTransform.position) < _attackReach)
            {
                StateMachine.ChangeState(new AttackState(Controller, _enemy.gameObject));
            }
        }

        private void ManualMove()
        {
            _moveInput.Normalize();
            float yDegree = Mathf.Atan2(_moveInput.x, _moveInput.y);
            var rotation = Quaternion.Euler(0, yDegree * Mathf.Rad2Deg, 0);
            var moveVector = new Vector3(_moveInput.x, 0, _moveInput.y) * (Controller._moveSpeed * Time.deltaTime);
            _characterTransform.SetPositionAndRotation(_characterTransform.position + moveVector, rotation);
        }
    }
}