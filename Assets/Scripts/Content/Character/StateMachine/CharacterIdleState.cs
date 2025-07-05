using Content.Enemy;
using UnityEngine;

namespace Content.Character.StateMachine
{
    public class CharacterIdleState : CharacterState
    {
        private float _timer;
        private const float DetectInterval = 1f;

        public CharacterIdleState(CharacterController controller)
            : base(controller)
        {
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > DetectInterval)
            {
                _timer = 0;
                Transform t = FindNearestEnemy();
                if (t.TryGetComponent<EnemyController>(out var component))
                {
                    StateMachine.ChangeState(new CharacterCombatState(Controller, component));
                }
            }
        }

        public Transform FindNearestEnemy()
        {
            Collider[] results = new Collider[] { };
            var size = Physics.OverlapSphereNonAlloc(Controller.transform.position, Controller.detectionRadius, results,
                Controller.enemyLayer);
            Transform nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var hit in results)
            {
                if (!hit.TryGetComponent<EnemyController>(out var component)) continue;

                float distance = Vector3.Distance(Controller.transform.position, hit.transform.position);
                if (distance < nearestDistance && !component.IsDead)
                {
                    nearest = hit.transform;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }
    }
}