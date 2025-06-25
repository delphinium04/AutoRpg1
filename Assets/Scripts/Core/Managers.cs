using System;
using UnityEngine;

namespace Core
{
    public class Managers : MonoBehaviour
    {
        private static Managers _instance;

        public static Managers Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindFirstObjectByType<Managers>();
                if (_instance == null)
                    Init();

                return _instance;
            }
        }

        #region Managers

        public static ResourceManager Resource { get; private set; }
        public static PoolManager Pool { get; private set; }

        #endregion

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private static void Init()
        {
            var go = new GameObject("@Managers");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<Managers>();

            Resource = new ResourceManager();
            Pool = new PoolManager();
        }

        private static void Clear()
        {
            Resource = null;
            Pool = null;
        }
    }
}