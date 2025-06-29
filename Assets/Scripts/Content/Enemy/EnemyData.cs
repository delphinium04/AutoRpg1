using UnityEngine;

namespace Content.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("Animation Clips")] public AnimatorOverrideController AnimatorController => _animatorOverrideController;
        [SerializeField] private AnimatorOverrideController _animatorOverrideController;
    }
}
