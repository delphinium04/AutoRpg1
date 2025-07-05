using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    [SerializeField] private Camera _mainCamera;
    private CharacterController _currentCharacter;
    private PlayerInput _playerInput;
    
    private void Awake()
    {
        _instance = this;
        _playerInput = GetComponent<PlayerInput>();
        // SetupInputSystem();
    }

    public void SetControlledCharacter(CharacterController character)
    {
        _currentCharacter = character;
        // FollowCamera.Instance.SetTarget(character.transform);
        
        // 자동/수동 모드 설정
        // _currentCharacter.SetControlMode(GameManager.IsAutoMode);
    }
}