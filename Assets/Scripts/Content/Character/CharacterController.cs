using System;
using UnityEngine;

namespace Content.Character
{
    [RequireComponent(typeof(CharacterInputHandler), typeof(Animator))]
    public class CharacterController : MonoBehaviour
    {
        // 추후 데이터화
        public float MoveSpeed = 3f;
        public CharacterData CharacterData;
        
        // SETTING
        public bool IsAutoMode = false;

        public CharacterInputHandler InputHandler { get; private set; }
        public CharacterStateMachine StateMachine { get; private set; }
        public Animator Animator { get; private set; }

        private void Awake()
        {
            InputHandler = GetComponent<CharacterInputHandler>();
            Animator = GetComponent<Animator>();
            StateMachine = new CharacterStateMachine();

            Init(CharacterData);
        }

        public void Init(CharacterData data)
        {
            CharacterData = data;
            Animator.runtimeAnimatorController = CharacterData.AnimatorController;
            
            StateMachine.Initialize(new CharacterIdleState(this));
        }

        private void Start()
        {
            StateMachine?.CurrentState?.Enter();
        }

        private void Update()
        {
            StateMachine?.CurrentState?.Update();
        }
    }
}