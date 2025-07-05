using UnityEngine;

namespace Content.Character
{
    public struct CharacterInit
    {
        public Datatable.CharacterSpecData Data;
        public int Level;
    }
    
    public abstract class CharacterAnimHash
    {
        public static readonly int MoveBool = Animator.StringToHash("IsMove");
        public static readonly int AttackedTrigger = Animator.StringToHash("Attacked");
        public static readonly int AttackTrigger = Animator.StringToHash("Attack");
        public static readonly int DieTrigger = Animator.StringToHash("Die");
    }
}