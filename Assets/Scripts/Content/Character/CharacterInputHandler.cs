using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Character
{
    [RequireComponent(typeof(PlayerInput))]
    public class CharacterInputHandler : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private InputAction _moveAction;

        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
        }

        private void OnEnable()
        {
            _moveAction.Enable();
        }

        private void Update()
        {
            MoveInput = _moveAction.ReadValue<Vector2>();
        }
    }
}