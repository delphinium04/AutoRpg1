using System;
using Core;
using Interfaces;
using UnityEngine;

namespace Content.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class MonsterController : MonoBehaviour, IPoolable
    {
        [SerializeField] private MonsterData _monsterData;

        private Animator _animator;

        private void Awake()
        {
            if (_monsterData == null)
            {
                gameObject.SetActive(false);
                Managers.Resource.Destroy(gameObject);
                throw new ArgumentNullException(nameof(_monsterData));
            }

            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            Initialize(_monsterData);
            OnSpawn();
        }


        #region interface

        /// 0: MonsterData
        public void Initialize(params object[] param)
        {
            if (param.Length == 0)
            {
                throw new ArgumentException("param length must not be 0");
            }

            if (param[0] is MonsterData monsterData)
            {
                _monsterData = monsterData;
            }
        }

        public void OnSpawn()
        {
            // _animator.rset
        }

        public void OnDespawn()
        {
            _monsterData = null;
        }

        #endregion
    }
}