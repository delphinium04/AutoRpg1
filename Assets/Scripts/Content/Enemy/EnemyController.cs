using System;
using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class EnemyController : MonoBehaviour
    {
        [FormerlySerializedAs("_monsterData")] [Header("Debug")] [SerializeField] private EnemyData enemyData;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            Init(enemyData);
        }
        
        public void Init(EnemyData data)
        {
            if (enemyData == null)
            {
                Managers.Resource.Destroy(gameObject);
                throw new ArgumentNullException(nameof(enemyData));
            }

            enemyData = data;
            _animator.runtimeAnimatorController = enemyData.AnimatorController;
        }
    }
}