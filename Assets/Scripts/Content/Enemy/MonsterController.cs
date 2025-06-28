using System;
using Core;
using UnityEngine;

namespace Content.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class MonsterController : MonoBehaviour
    {
        [Header("Debug")] [SerializeField] private MonsterData _monsterData;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            Init(_monsterData);
        }
        
        public void Init(MonsterData data)
        {
            if (_monsterData == null)
            {
                Managers.Resource.Destroy(gameObject);
                throw new ArgumentNullException(nameof(_monsterData));
            }

            _monsterData = data;
            _animator.runtimeAnimatorController = _monsterData.AnimatorController;
        }
    }
}