using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public class ResourceManager
    {
        // need cache util
        public T Load<T>(string path = "") where T : Object
        {
            // if (typeof(T) != typeof(GameObject)) 
            return Resources.Load<T>(path);

            // var prefabName = path;
            // if (path.LastIndexOf('/') != -1)
            // {
            //     prefabName = path[(path.LastIndexOf('/') + 1)..];
            // }
            //
            // var prefab = Managers.Pool.GetOriginal(prefabName);
            // if (prefab is not null)
            //     return prefab as T;
            //
            // return Resources.Load<T>(path);
        }


        public GameObject Instantiate(string path = "")
        {
            var original = Load<GameObject>(path);
            if (original is null)
            {
                Logging.Write($"Resource not found: {path}", Logging.LogLevel.Warning);
                return null;
            }

            return original.TryGetComponent<Poolable>(out var poolable)
                ? Managers.Pool.Get(poolable)
                : Object.Instantiate(original);
        }

        public void Destroy(GameObject target)
        {
            if (!target)
            {
                Logging.Write("Target was already destroyed", Logging.LogLevel.Warning);
                return;
            }

            // Pool
            if (target.TryGetComponent<Poolable>(out var poolable))
            {
                Managers.Pool.Destroy(poolable);
                return;
            }

            Object.Destroy(target);
        }
    }
}