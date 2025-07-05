using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _currentCharacter;
    
    // Input Action들을 위한 콜백 참조
    private InputAction _moveAction;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        SetupInputActions();
    }
    
    private void SetupInputActions()
    {
        // Input Action Map에서 액션들 가져오기
        _moveAction = _playerInput.actions["Move"];
        
        // 콜백 등록
        _moveAction.performed += OnMove;
    }
    
    // 현재 제어 중인 캐릭터 설정
    public void SetActiveCharacter(CharacterController character)
    {
        _currentCharacter = character;
    }
    
    private void OnMove(InputAction.CallbackContext context)
    {
        if (_currentCharacter != null)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            // _currentCharacter.HandleMovement(moveInput);
        }
    }
    
    private void OnDisable()
    {
        // 콜백 해제
        _moveAction.performed -= OnMove;
    }
}