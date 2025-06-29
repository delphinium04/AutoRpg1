using System;
using Content.Character.StateMachine;
using UnityEngine;

namespace Content.Character
{
    [RequireComponent(typeof(CharacterInputHandler), typeof(Animator))]
    public class CharacterController : MonoBehaviour
    {
        // 추후 데이터화
        public float _moveSpeed = 3f;
        public CharacterData _characterData;
        
        // SETTING
        public bool _isAutoMode = true;

        public CharacterInputHandler InputHandler { get; private set; }
        public CharacterStateMachine StateMachine { get; private set; }
        public Animator Animator { get; private set; }

        private void Awake()
        {
            InputHandler = GetComponent<CharacterInputHandler>();
            Animator = GetComponent<Animator>();
            StateMachine = new CharacterStateMachine();

            Init(_characterData);
        }

        public void Init(CharacterData data)
        {
            _characterData = data;
            Animator.runtimeAnimatorController = _characterData.AnimatorController;
            
            StateMachine.Initialize(new IdleState(this));
        }

        private void Start()
        {
            StateMachine?.CurrentState?.Enter();
        }

        private void Update()
        {
            StateMachine?.CurrentState?.Update();

            if (InputHandler.MoveInput.magnitude > 0.1f)
                _isAutoMode = false;
        }
    }
}