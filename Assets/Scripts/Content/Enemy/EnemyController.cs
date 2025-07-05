using System;
using Content.Enemy.StateMachine;
using Core;
using UnityEngine;
using CharacterController = Content.Character.CharacterController;

namespace Content.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Debug")] [SerializeField] private AnimatorOverrideController _animatorOverrideController;

        public EnemyStateMachine StateMachine { get; private set; }
        public Animator Animator { get; private set; }

        public float PlayerDetectionReach { get; private set; } = 7f;
        public float AttackReach { get; private set; } = 2f;

        public bool IsDead { get; private set; }
        public int Health { get; private set; } = 10000;

        public CharacterController TargetCharacter = null; 

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            StateMachine = new EnemyStateMachine();
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            Animator.runtimeAnimatorController = _animatorOverrideController;

            Health = 100;
            gameObject.GetComponent<Collider>().enabled = true;

            StateMachine.Initialize(new EnemyIdleState(this));
        }

        private void Update()
        {
            StateMachine?.CurrentState?.Update();
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsDead = true;
                gameObject.GetComponent<Collider>().enabled = false;
                Animator.SetTrigger(EnemyAnimHash.DieTrigger);
                Invoke(nameof(Die), 1f);
            }
        }

        public void Die()
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}