using UnityEngine;

namespace Content.Character
{
    public class CharacterController : MonoBehaviour
    {
        private float _moveSpeed = 3f;
        private Vector3 _input = Vector3.zero;

        private void Awake()
        {
            var inputHandler = GetComponent<CharacterInputHandler>();
            if (inputHandler != null)
            {
                inputHandler.OnMoveEvent += UpdateMovementInput;
                // inputHandler.OnAttackKeyPressed += TryAttack;
            }
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector3 moveDirection = _input * (Time.deltaTime * _moveSpeed);
            transform.Translate(moveDirection);
        }

        private void UpdateMovementInput(Vector2 input)
        {
            _input = new Vector3(input.x, 0, input.y);
            if(_input.magnitude > 1) _input.Normalize();
        }

        private void TryAttack()
        {
            Debug.Log("Attack");
        }
    }
}