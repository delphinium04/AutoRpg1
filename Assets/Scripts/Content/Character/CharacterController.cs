using Content.Character.StateMachine;
using Content.Enemy;
using UnityEngine;

namespace Content.Character
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Components")] public CharacterInputHandler InputHandler { get; private set; }
        public CharacterStateMachine StateMachine { get; private set; }
        public CharacterData Data { get; private set; }
        public Animator Animator { get; private set; }

        [Header("Detection")] public float DetectionRadius { get; private set; }= 10f;
        public LayerMask EnemyLayer;

        [Header("Variables")] public EnemyController TargetEnemy;

        [Header("Debug")] public AnimatorOverrideController animatorOverrideController;
        public bool Initiator = true;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            StateMachine = new CharacterStateMachine();
            InputHandler = GetComponent<CharacterInputHandler>();
        }

        private void Start()
        {
            if (Initiator)
                Init();
        }

        public void Init()
        {
            gameObject.SetActive(true);
            Data = new CharacterData();
            Animator.runtimeAnimatorController = animatorOverrideController; //DEBUG

            StateMachine.ChangeState(new CharacterIdleState(this));
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