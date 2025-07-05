using Content.Character.StateMachine;
using Datatable;
using UnityEngine;

namespace Content.Character
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Components")] public CharacterInputHandler InputHandler { get; private set; }
        public CharacterStateMachine StateMachine { get; private set; }
        public CharacterData Data { get; private set; }
        public Animator Animator { get; private set; }

        [Header("Detection")] public float detectionRadius = 10f;
        public LayerMask enemyLayer;

        [Header("Debug")] public AnimatorOverrideController animatorOverrideController;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            StateMachine = new CharacterStateMachine();
            InputHandler = GetComponent<CharacterInputHandler>();

            // DEBUG
            Animator.runtimeAnimatorController = animatorOverrideController;
            CharacterInit init = new CharacterInit
            {
                Data = CharacterSpecDataProvider.Get(4),
                Level = 4
            };
            Init(init);
        }

        public void Init(CharacterInit init)
        {
            Data = new CharacterData(init);
        }

        private void Start()
        {
            StateMachine?.CurrentState?.Enter();
        }

        private void Update()
        {
            StateMachine?.CurrentState?.Update();
        }


        public void TakeDamage(int damage)
        {
            if (Data.Hp <= 0) return;

            Data.Hp -= damage;
            if (Data.Hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            StateMachine.ChangeState(new CharacterDeadState(this));
        }
    }
}