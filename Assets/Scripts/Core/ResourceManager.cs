using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    /*
    - 리소스 캐싱 시스템 구현 -> Ok
    - 적절한 예외 처리 추가 -> 어디에 어떻게?
    - 경로 유효성 검사 로직 추가 -> OK("" X)
    - 리소스 해제 타이밍 최적화 -> ?
     */
    public class ResourceManager
    {
        Dictionary<string, Object> _loadCache = new Dictionary<string, Object>();
        HashSet<string> _pooledPrefabPaths = new HashSet<string>(); // Pool에서 null 참조 방지용

        public T Load<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Logging.Write("path is null or empty", Logging.LogLevel.Warning);
                return null;
            }

            if (_loadCache.TryGetValue(path, out var cached))
            {
                if (cached is T value)
                    return value;

                _loadCache.Remove(path);
            }

            try
            {
                var asset = Resources.Load<T>(path);
                if (asset)
                    _loadCache.Add(path, asset);
                else
                    Logging.Write($"{path} is invalid", Logging.LogLevel.Warning);
                return asset;
            }
            catch (Exception e)
            {
                Logging.Write($"Error loading {path}: {e.Message}", Logging.LogLevel.Error);
                throw;
            }
        }

        public GameObject Instantiate(string path, Transform parent = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logging.Write("path is null or empty", Logging.LogLevel.Warning);
                return null;
            }

            var original = Load<GameObject>(path);
            if (!original)
            {
                Logging.Write($"Resource not found: {path}", Logging.LogLevel.Warning);
                return null;
            }

            GameObject gameObject;
            if (original.TryGetComponent<Poolable>(out var poolable))
            {
                gameObject = Managers.Pool.Get(poolable);
                _pooledPrefabPaths.Add(path);
            }
            else gameObject = Object.Instantiate(original);

            if (gameObject && parent)
                gameObject.transform.SetParent(parent);

            return gameObject;
        }

        public void Destroy(GameObject target)
        {
            if (!target)
            {
                Logging.Write("Target was already destroyed", Logging.LogLevel.Warning);
                return;
            }

            // if the target is poolable
            if (target.TryGetComponent<Poolable>(out var poolable))
            {
                Managers.Pool.Destroy(poolable);
                return;
            }

            Object.Destroy(target);
        }

        public void Clear()
        {
            var keys = new List<string>(_loadCache.Keys);
            foreach (var key in keys)
            {
                // 풀링된 오브젝트는 PoolManager에서 관리
                if (_pooledPrefabPaths.Contains(key))
                    continue;

                var asset = _loadCache[key];
                if (asset is not (not GameObject or Component)) continue;
                try
                {
                    Resources.UnloadAsset(asset);
                }
                catch (Exception e)
                {
                    Logging.Write($"Error unloading {key}: {e.Message}", Logging.LogLevel.Error);
                    throw;
                }
            }

            _loadCache.Clear();
            _pooledPrefabPaths.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}