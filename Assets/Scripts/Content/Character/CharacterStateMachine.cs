// CharacterStateMachine.cs

using UnityEngine;

namespace Content.Character
{
    public enum CharacterStateType
    {
        Idle,
        Move,
        Attack,
        Die
    }

    public abstract class CharacterState
    {
        protected CharacterController Controller;
        protected CharacterStateMachine StateMachine;

        protected CharacterState(CharacterController controller, CharacterStateMachine stateMachine)
        {
            Controller = controller;
            StateMachine = stateMachine;
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

        public virtual void FixedUpdate()
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

    public class IdleState : CharacterState
    {
        /*
         * 플레이어가 이동 조작 시 -> MoveState
         * 자동 이동 기능 On + 적 발견 -> MoveState
         * 적을 넘기는 방법에 대해선 고민 (수동이동때문에)
         */
        
        
        private float _searchInterval = 0.5f;
        private float _nextSearchTime;

        public IdleState(CharacterController controller, CharacterStateMachine stateMachine)
            : base(controller, stateMachine)
        {
        }

        public override void Enter()
        {
            _nextSearchTime = 0f;
        }

        // TODO: searching enemy
        public override void Update()
        {
            if (Controller.InputHandler.MoveInput.magnitude > 0.1f)
            {
                StateMachine.ChangeState(new MoveState(Controller, StateMachine));
                return;
            }
        }
        
        // TODO: 알고리즘 사용 필요?
        public Transform FindNearestTarget(Vector3 position, string targetTag = "Enemy")
        {
            var enemies = GameObject.FindGameObjectsWithTag(targetTag);
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;
        
            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = enemy.transform;
                }
            }

            return closestTarget;
        }

    }

    public class MoveState : CharacterState
    {
        /*
         * 적이 공격 거리 내 들어온 경우 -> Attack
         * 
         */

        private Transform controllerT;
        
        public MoveState(CharacterController controller, CharacterStateMachine stateMachine)
            : base(controller, stateMachine)
        {
        }
        
        public override void Update()
        {
            // 수동 이동 전제
            var moveInput = Controller.InputHandler.MoveInput;
        }
    }

    public class AttackState : CharacterState
    {
        private readonly float _attackRange = 2f;
        private readonly float _attackDelay = 1f;
        private float _nextAttackTime;

        public AttackState(CharacterController controller, CharacterStateMachine stateMachine, Transform target)
            : base(controller, stateMachine)
        {
        }

        public override void Enter()
        {
            _nextAttackTime = 0f;
        }

        public override void Update()
        {
            // float distanceToTarget = Vector3.Distance(Controller.transform.position, Target.position);
            //
            // if (distanceToTarget > _attackRange)
            // {
            //     StateMachine.ChangeState(new MoveState(Controller, StateMachine, Target));
            //     return;
            // }
            //
            // if (Time.time >= _nextAttackTime)
            // {
            //     Controller.Animator.SetTrigger(AttackTriggerHash);
            //     _nextAttackTime = Time.time + _attackDelay;
            //
            //     // 데미지 처리
            //     if (Target.TryGetComponent<IDamageable>(out var damageable))
            //     {
            //         damageable.TakeDamage(Controller.AttackDamage);
            //     }
            // }
        }
    }

    public class DeathState : CharacterState
    {
        public DeathState(CharacterController controller, CharacterStateMachine stateMachine)
            : base(controller, stateMachine)
        {
        }

        public override void Enter()
        {
            // Controller.Animator.SetTrigger(DeathTriggerHash);
        }
    }
}