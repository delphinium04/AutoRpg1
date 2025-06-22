using UnityEngine;

namespace Content.Enemy
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Data/Monster")]
    public class MonsterData : ScriptableObject
    {
        [Header("Animation Clips")] public AnimatorOverrideController AnimatorController => _animatorOverrideController;
        [SerializeField] private AnimatorOverrideController _animatorOverrideController;
    }
}
