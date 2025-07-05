using Content.Character;
using UnityEngine;
using CharacterController = UnityEngine.CharacterController;

namespace Content.Enemy.StateMachine
{
    public class MoveState : EnemyState
    {
        /*
         * (수동) 이동을 멈출 경우 -> Idle
         * (자동) 적이 공격 거리 내 들어온 경우 -> Attack
         */
        private GameObject _target;

        private Vector2 _moveInput;
        private Transform _transform;
        private Transform _characterTransform;

        public MoveState(EnemyController controller, GameObject target = null)
            : base(controller)
        {
            _target = target;
        }

        public override void Enter()
        {
            if (!_target)
            {
                StateMachine.ChangeState(new IdleState(Controller));
                return;
            }

            _transform = Controller.transform;
            _characterTransform = _target.transform;

            Animator.SetBool(EnemyAnimHash.MoveBool, true);
        }

        public override void Update()
        {
            AutoMove();
        }

        public override void Exit()
        {
            Animator.SetBool(CharacterAnimHash.MoveBool, false);
        }

        private void AutoMove()
        {
            if (Vector3.Distance(_transform.position, _characterTransform.position) < Controller.AttackReach)
            {
                StateMachine.ChangeState(new AttackState(Controller, null));
            }

            Vector3 direction = (_characterTransform.position - _transform.position).normalized;
            Vector3 moveVec = direction * (2 * Time.deltaTime);
            Quaternion rotation = Quaternion.LookRotation(direction);
            _transform.SetPositionAndRotation(_transform.position + moveVec, rotation);
        }
    }
}