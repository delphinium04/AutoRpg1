using System;
using UnityEngine;

namespace Core
{
    public class Poolable : MonoBehaviour
    {
        public bool IsActive { get; set; }
        public event Action OnSpawned = null;
        public event Action OnDespawned = null;

        public void OnSpawn()
        {
            OnSpawned?.Invoke();
        }

        public void OnDespawn()
        {
            OnDespawned?.Invoke();
        }
    }
}