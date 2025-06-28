using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField] private string prefabHash;
        public string PrefabHash => prefabHash;
    }
}