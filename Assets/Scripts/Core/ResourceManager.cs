using UnityEngine;

namespace Core
{
    public class ResourceManager
    {
        public Object Instantiate(UnityEngine.Object prefab, Transform root = null, bool worldPositionStays = false,
            Vector3 position = default, Quaternion rotation = default)
        {
            // Pool
            var instantiatedObject = UnityEngine.Object.Instantiate(prefab);

            return instantiatedObject;
        }
        
        public void Destroy(UnityEngine.Object target){
            // Pool
            UnityEngine.Object.Destroy(target);
        } 
    }
}