using UnityEngine;

namespace Content.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Character")]
    public class CharacterData : ScriptableObject
    {
        [Header("Animation Clips")] public AnimatorOverrideController AnimatorController => _animatorOverrideController;
        [SerializeField] private AnimatorOverrideController _animatorOverrideController;
    }
}
