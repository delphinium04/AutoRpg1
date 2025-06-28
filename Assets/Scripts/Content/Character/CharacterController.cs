using System;
using UnityEngine;

namespace Content.Character
{
    [RequireComponent(typeof(CharacterInputHandler), typeof(Animator))]
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;

        public CharacterInputHandler InputHandler { get; private set; }
        public CharacterStateMachine StateMachine { get; private set; }
        public Animator Animator { get; private set; }

        private void Awake()
        {
            InputHandler = GetComponent<CharacterInputHandler>();
            Animator = GetComponent<Animator>();
            StateMachine = new CharacterStateMachine();
        }

        private void Update()
        {
            
        }
    }
}