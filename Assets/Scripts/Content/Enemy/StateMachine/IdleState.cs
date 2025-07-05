using Core;
using UnityEngine;

namespace Content.Enemy.StateMachine
{
    public class IdleState : EnemyState
    {
        private Collider[] _result;

        private float _timer;

        public IdleState(EnemyController controller)
            : base(controller)
        {
        }

        public override void Enter()
        {
            _result = new Collider[1];
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > 1)
            {
                CheckNearbyPlayer();
                _timer = 0;
            }
        }

        public void CheckNearbyPlayer()
        {
            Logging.Write("Pulse");
            if (Physics.OverlapSphereNonAlloc(Controller.transform.position, Controller.PlayerDetectionReach, _result,
                    LayerMask.GetMask("Player")) <= 0) return;
            StateMachine.ChangeState(new MoveState(Controller, _result[0].gameObject));
            Logging.Write(_result[0].gameObject.name);
        }
    }
}