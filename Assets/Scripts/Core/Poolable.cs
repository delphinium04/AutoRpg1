using System.Linq;
using UnityEngine;

namespace Core
{
    public class Poolable : MonoBehaviour
    {
        private string _id;

        public string ID
        {
            get
            {
                if (!string.IsNullOrEmpty(_id)) return _id;

                // 가진 컴포넌트와 오브젝트 이름을 이용한 Hash 생성
                string cleanName = gameObject.name.Replace("(Clone)", "").Trim();
                var components = GetComponents<Component>()
                    .Select(c => c.GetType().Name)
                    .OrderBy(n => n);
                string componentKey = string.Join("_", components);
                _id = $"{cleanName}_{componentKey}".GetHashCode().ToString();
                return _id;
            }
        }
    }
}