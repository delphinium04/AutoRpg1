using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Core
{
    public class PoolManager
    {
        private Transform RootTransform { get; set; }

        // DISPOSE 추가 필요
        private class Pool
        {
            public GameObject Original { get; }
            private Transform _root;
            private Stack<GameObject> _pool;

            private const int PoolIncreasement = 10;

            public Pool(GameObject prefab)
            {
                Original = prefab;
                _root = new GameObject($"Root_{prefab.name}").transform;
                _root.SetParent(Managers.Pool.RootTransform);
                _pool = new Stack<GameObject>();

                Create();
            }

            ~Pool()
            {
                foreach (var gameObject in _pool)
                {
                    Object.Destroy(gameObject);
                }

                Object.Destroy(_root.gameObject);
                _pool.Clear();
            }

            private void Create()
            {
                for (int i = 0; i < PoolIncreasement; i++)
                {
                    var go = Object.Instantiate(Original, _root);
                    go.SetActive(false);
                    _pool.Push(go);
                }
            }

            public GameObject Pop()
            {
                if (_pool.Count == 0)
                    Create();

                return _pool.Pop();
            }

            public void Push(GameObject obj)
            {
                _pool.Push(obj);
            }
        }

        private Dictionary<string, Pool> _poolDict;

        public PoolManager()
        {
            _poolDict = new Dictionary<string, Pool>();

            Transform rootGo = new GameObject("@Pool").transform;
            rootGo.SetParent(Managers.Instance.transform);
            RootTransform = rootGo;
        }

        ~PoolManager()
        {
            var pools = _poolDict.Values.ToList();
            for (int i = 0; i < pools.Count; i++)
            {
                pools[i] = null;
            }

            _poolDict.Clear();

            if (RootTransform)
                Object.Destroy(RootTransform.gameObject);
        }

        private void Create(Poolable poolable)
        {
            string key = poolable.PrefabHash;
            var pool = new Pool(poolable.gameObject);
            _poolDict.Add(key, pool);
        }

        public GameObject Get(Poolable poolable)
        {
            var key = poolable.PrefabHash;
            if (!_poolDict.ContainsKey(key))
                Create(poolable);
            
            var go = _poolDict[key].Pop();
            go.SetActive(true);
            
            return go;
        }

        // return prefab if pool dictionary contains has 'name' key
        // public GameObject GetOriginal(string name)
        // {
        //     return _poolDict.GetValueOrDefault(name)?.Original;
        // }

        public void Destroy(Poolable poolObject)
        {
            if (_poolDict.TryGetValue(poolObject.PrefabHash, out var pool))
            {
                poolObject.gameObject.SetActive(false);
                pool.Push(poolObject.gameObject);
                return;
            }

            Logging.Write($"{poolObject.name} has no pool, Destroy");
            Object.Destroy(poolObject.gameObject);
        }
    }
}