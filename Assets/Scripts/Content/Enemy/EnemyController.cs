using System;
using Content.Enemy.StateMachine;
using Core;
using UnityEngine;

namespace Content.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Debug")] [SerializeField] private EnemyData _enemyData;

        public EnemyStateMachine StateMachine { get; private set; }
        public Animator Animator { get; private set; }

        public float PlayerDetectionReach { get; private set; } = 7f;
        public float AttackReach { get; private set; } = 2f;

        public bool IsDead { get; private set; }
        public int Health { get; private set; } = 100;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            StateMachine = new EnemyStateMachine();
        }

        private void Start()
        {
            Init(_enemyData);
        }

        public void Init(EnemyData data)
        {
            if (_enemyData == null)
            {
                Managers.Resource.Destroy(gameObject);
                throw new ArgumentNullException(nameof(_enemyData));
            }

            _enemyData = data;
            Animator.runtimeAnimatorController = _enemyData.AnimatorController;

            Health = 100;
            gameObject.GetComponent<Collider>().enabled = true;

            StateMachine.Initialize(new IdleState(this));
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