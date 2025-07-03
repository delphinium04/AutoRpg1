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
        public float PlayerDetectionReach { get; private set; } = 7f;

        private Animator _animator;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
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
            _animator.runtimeAnimatorController = _enemyData.AnimatorController;

            StateMachine.Initialize(new IdleState(this));
        }
    }
}