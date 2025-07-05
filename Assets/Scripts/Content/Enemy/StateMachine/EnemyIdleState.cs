using Core;
using UnityEngine;

namespace Content.Enemy.StateMachine
{
    public class EnemyIdleState : EnemyState
    {
        private Collider[] _result = new Collider[1];

        private float _timer;
        private float _searchInterval = 1f;

        public EnemyIdleState(EnemyController controller)
            : base(controller)
        {
        }

        public override void Enter()
        {
            Controller.Animator.SetBool(EnemyAnimHash.MoveBool, false);;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > _searchInterval)
            {
                CheckNearbyPlayer();
                _timer = 0;
            }
        }

        public void CheckNearbyPlayer()
        {
            if (Physics.OverlapSphereNonAlloc(Controller.transform.position, Controller.PlayerDetectionReach, _result,
                    LayerMask.GetMask("Player")) <= 0) return;
            
            if (_result[0]?.TryGetComponent<Content.Character.CharacterController>(out var character) ?? false)
            {
                Controller.TargetCharacter = character;
                StateMachine.ChangeState(new EnemyCombatState(Controller));
            }
        }
    }
}