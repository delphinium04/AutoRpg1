using UnityEngine;

namespace Content.Enemy
{
    public abstract class EnemyAnimHash
    {
        public static readonly int MoveBool = Animator.StringToHash("IsMove");
        public static readonly int AttackedTrigger = Animator.StringToHash("Attacked");
        public static readonly int AttackTrigger = Animator.StringToHash("Attack");
        public static readonly int DieTrigger = Animator.StringToHash("Die");
    }
}