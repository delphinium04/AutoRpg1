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
            public GameObject Original { get; private set; }
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

        private void Create(GameObject prefab)
        {
            string key = prefab.name;
            var pool = new Pool(prefab);
            _poolDict.Add(key, pool);
        }

        public GameObject Get(GameObject prefab)
        {
            if (!_poolDict.ContainsKey(prefab.name))
                Create(prefab);

            return _poolDict[prefab.name].Pop();
        }

        // return prefab if pool dictionary contains has 'name' key
        public GameObject GetOriginal(string name)
        {
            return _poolDict.GetValueOrDefault(name)?.Original;
        }

        public void Destroy(GameObject gameObject)
        {
            if (_poolDict.TryGetValue(gameObject.name, out var pool))
            {
                pool.Push(gameObject);
                return;
            }

            Logging.Write($"{gameObject.name} has no pool, Destroy");
            Object.Destroy(gameObject);
        }
    }
}