using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMoveEvent = null;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        playerInput.onActionTriggered += OnActionTriggered;
    }

    #region PlayerInput

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                HandleMove(context);
                break;
        }
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                Vector2 moveValue = context.ReadValue<Vector2>();
                Debug.Log($"Move Performed: {moveValue}");
                OnMoveEvent?.Invoke(moveValue);
                break;
            case InputActionPhase.Canceled:
                Debug.Log("Move Canceled");
                OnMoveEvent?.Invoke(Vector2.zero);
                break;
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.onActionTriggered -= OnActionTriggered;
    }

    #endregion
}