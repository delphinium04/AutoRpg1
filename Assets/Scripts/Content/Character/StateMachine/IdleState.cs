using UnityEngine;

namespace Content.Character.StateMachine
{
    public class IdleState : CharacterState
    {
        /*
         * 플레이어가 이동 조작 시 -> MoveState
         * 자동 기능 + 적 발견 -> Move
         */

        private const float CheckInterval = 0.5f;
        private float _checkTimer;

        public IdleState(CharacterController controller)
            : base(controller)
        {
        }

        public override void Update()
        {
            _checkTimer += Time.deltaTime;

            if (Controller._isAutoMode && _checkTimer > CheckInterval)
            {
                _checkTimer = 0;
                GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
                if (enemy)
                    StateMachine.ChangeState(new MoveState(Controller, enemy));
            }

            if (Controller.InputHandler.MoveInput.magnitude > 0.1f)
            {
                StateMachine.ChangeState(new MoveState(Controller));
            }
        }
    }
}