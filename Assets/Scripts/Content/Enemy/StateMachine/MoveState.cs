using Content.Character.Common;
using Content.Character.StateMachine;
using UnityEngine;

namespace Content.Enemy.StateMachine
{
    public class MoveState : EnemyState
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

        public MoveState(EnemyController controller, GameObject target = null)
            : base(controller)
        {
        }

        public override void Enter()
        {
            // Animator.SetBool(CharacterAnimHash.MoveBool, true);
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            Animator.SetBool(CharacterAnimHash.MoveBool, false);
        }

        private void AutoMove()
        {
            if (Vector3.Distance(_characterTransform.position, _enemyTransform.position) < _attackReach)
            {
                StateMachine.ChangeState(new AttackState(Controller, null));
            }
        }
    }
}