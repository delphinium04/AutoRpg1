using Content.Enemy;
using UnityEngine;

namespace Content.Character.StateMachine
{
    public class CharacterIdleState : CharacterState
    {
        private float _timer;
        private const float DetectInterval = 1f;
        private Collider[] results = new Collider[5];

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
                if (t?.TryGetComponent<EnemyController>(out var component) != true) return;

                Controller.TargetEnemy = component;
                StateMachine.ChangeState(new CharacterCombatState(Controller));
            }
        }

        public Transform FindNearestEnemy()
        {
            var size = Physics.OverlapSphereNonAlloc(Controller.transform.position, Controller.DetectionRadius, results,
                Controller.EnemyLayer);
            Transform nearest = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < size; i++)
            {
                var hit = results[i];
                if (!hit.TryGetComponent<EnemyController>(out var component))
                    continue;

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