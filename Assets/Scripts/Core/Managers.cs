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
                        Init();
                }

                return _instance;
            }
        }

        #region Managers

        public static ResourceManager Resource { get; private set; }

        #endregion
        
        private static void Init()
        {
            var go = new GameObject("@Managers");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<Managers>();

            Resource = new ResourceManager();
        }
    }
}