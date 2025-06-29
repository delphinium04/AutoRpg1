using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core
{
    public class Pool : IDisposable
    {
        private bool _isDisposed = false;

        public GameObject Original { get; }

        private Transform _root;
        private Stack<GameObject> _pool;
        private const int GrowthSize = 5;

        public Pool(GameObject prefab)
        {
            Original = prefab;
            _root = new GameObject($"Root_{prefab.name}").transform;
            _root.SetParent(Managers.Pool.Root);
            _pool = new Stack<GameObject>();

            Create();
        }

        ~Pool()
        {
            ReleaseUnmanagedResources();
        }

        // TODO: Coroutine을 활용해서 부하 적게 다시 구현
        private void Create()
        {
            for (int i = 0; i < GrowthSize; i++)
            {
                var go = Object.Instantiate(Original, _root);
                go.SetActive(false);
                _pool.Push(go);
            }
        }

        // TODO: pop 직후 SetActive 더 안전하게?   
        public GameObject Pop()
        {
            if (_pool.Count == 0)
            {
                Create();
            }

            var obj = _pool.Pop();
            obj.SetActive(true);
            return obj;
        }

        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Push(obj);
        }

        private void ReleaseUnmanagedResources()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Object.Destroy(Original);
            Object.Destroy(_root.gameObject);
            foreach (var gameObject in _pool)
            {
                Object.Destroy(gameObject);
            }

            _pool.Clear();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }

    public class PoolManager
    {
        public Transform Root { get; }

        private Dictionary<string, Pool> _poolDict;

        public PoolManager()
        {
            _poolDict = new Dictionary<string, Pool>();

            Transform poolRoot = Managers.Instance.transform.Find("@Pool");
            if (!poolRoot)
                poolRoot = new GameObject("@Pool").transform;
            poolRoot.SetParent(Managers.Instance.transform);
            Root = poolRoot;
        }

        public void Clear()
        {
            foreach (var pool in _poolDict.Values)
            {
                pool.Dispose();
            }

            _poolDict.Clear();
        }

        public GameObject Get(Poolable poolable)
        {
            if (!poolable) return null;

            var key = poolable.ID;
            if (!_poolDict.ContainsKey(key))
                CreatePair(poolable);

            var go = _poolDict[key].Pop();
            go.SetActive(true);
            return go;
        }

        public void Destroy(Poolable poolable)
        {
            if (!poolable) return;

            if (_poolDict.TryGetValue(poolable.ID, out var pool))
            {
                poolable.gameObject.SetActive(false);
                pool.Push(poolable.gameObject);
                return;
            }

            Logging.Write($"No pool of {poolable.name} in dict, just destroy");
            Object.Destroy(poolable.gameObject);
        }

        private void CreatePair(Poolable poolable)
        {
            var key = poolable.ID;
            var pool = new Pool(poolable.gameObject);
            _poolDict.Add(key, pool);
        }
    }
}