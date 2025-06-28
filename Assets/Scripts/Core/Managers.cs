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
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<Managers>();

                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(Managers).Name);
                        _instance = obj.AddComponent<Managers>();
                    }
                }

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

            Resource = new ResourceManager();
            Pool = new PoolManager();
        }
    }
}