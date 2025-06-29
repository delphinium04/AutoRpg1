// CharacterStateMachine.cs

using Content.Enemy;
using Core;
using UnityEngine;

namespace Content.Character
{
    public abstract class CharacterAnimHash
    {
        public static readonly int MoveBool = Animator.StringToHash("IsMove");
        public static readonly int AttackedTrigger = Animator.StringToHash("Attacked");
        public static readonly int AttackTrigger = Animator.StringToHash("Attack");
        public static readonly int DieTrigger = Animator.StringToHash("Die");
    }

    public abstract class CharacterState
    {
        protected CharacterController Controller;
        protected CharacterStateMachine StateMachine;
        protected CharacterInputHandler InputHandler;
        protected Animator Animator;

        protected CharacterState(CharacterController controller)
        {
            Controller = controller;
            StateMachine = Controller.StateMachine;
            InputHandler = Controller.InputHandler;
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

    public class CharacterStateMachine
    {
        public CharacterState CurrentState { get; private set; }

        public void Initialize(CharacterState state)
        {
            CurrentState = state;
            CurrentState.Enter();
        }

        public void ChangeState(CharacterState state)
        {
            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }

    public class CharacterIdleState : CharacterState
    {
        /*
         * 플레이어가 이동 조작 시 -> MoveState
         * 자동 기능 + 적 발견 -> Move
         */

        private const float CheckInterval = 0.5f;
        private float _checkTimer = 0;

        public CharacterIdleState(CharacterController controller)
            : base(controller)
        {
        }

        public override void Update()
        {
            _checkTimer += Time.deltaTime;

            if (Controller.IsAutoMode && _checkTimer > CheckInterval)
            {
                _checkTimer = 0;
                GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
                if (enemy)
                    StateMachine.ChangeState(new CharacterMoveState(Controller, enemy));
            }

            if (Controller.InputHandler.MoveInput.magnitude > 0.1f)
            {
                StateMachine.ChangeState(new CharacterMoveState(Controller));
            }
        }
    }

    public class CharacterMoveState : CharacterState
    {
        /*
         * (수동) 이동을 멈출 경우 -> Idle
         * (자동) 적이 공격 거리 내 들어온 경우 -> Attack
         */

        private Vector2 _moveInput;
        private Transform _characterTransform;

        private EnemyController _enemy;
        private Transform _enemyTransform;

        public CharacterMoveState(CharacterController controller, GameObject target = null)
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
            if (Controller.IsAutoMode)
            {
                AutoMove();
                return;
            }

            // 수동 이동
            _moveInput = InputHandler.MoveInput;
            if (_moveInput.sqrMagnitude < 0.1f)
            {
                StateMachine.ChangeState(new CharacterIdleState(Controller));
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
            _characterTransform.Translate(Vector3.forward * (Controller.MoveSpeed * Time.deltaTime));

            if (Vector3.Distance(_characterTransform.position, _enemyTransform.position) < 5f)
            {
                StateMachine.ChangeState(new AttackState(Controller, _enemy.gameObject));
            }
        }

        private void ManualMove()
        {
            _moveInput.Normalize();
            float yDegree = Mathf.Atan2(_moveInput.x, _moveInput.y);
            var rotation = Quaternion.Euler(0, yDegree * Mathf.Rad2Deg, 0);
            var moveVector = new Vector3(_moveInput.x, 0, _moveInput.y) * (Controller.MoveSpeed * Time.deltaTime);
            _characterTransform.SetPositionAndRotation(_characterTransform.position + moveVector, rotation);
        }
    }

    public class AttackState : CharacterState
    {
        // 공격 후 다시 Idle로

        private float _debugTimer = 0f;

        public AttackState(CharacterController controller, GameObject target)
            : base(controller)
        {
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
                StateMachine.ChangeState(new CharacterIdleState(Controller));
            }
        }
    }
}