using Content.Character;
using Content.Character.StateMachine;
using Content.Enemy;
using Core;
using UnityEngine;
using CharacterController = Content.Character.CharacterController;

public class CharacterCombatState : CharacterState
{
    private EnemyController _target;
    private Vector3 _positionDelta;

    private float _moveSpeed = 3;
    private float _lastAttackTime = 0;
    private float _attackInterval = 1;

    public CharacterCombatState(CharacterController controller, EnemyController enemy = null) : base(controller)
    {
        _target = enemy;
    }

    public override void Enter()
    {
        if (!_target)
        {
            Logging.Write("No target, change to idleState", Logging.LogLevel.Warning);
            StateMachine.ChangeState(new CharacterIdleState(Controller));
        }

        _attackInterval = 1 / Mathf.Min(Controller.Data.AttackSpeed, 0.01f);
        Animator.SetBool(CharacterAnimHash.MoveBool, true);
    }

    public override void Exit()
    {
        Animator.SetBool(CharacterAnimHash.MoveBool, false);
    }

    public override void Update()
    {
        if (_target.IsDead)
        {
            StateMachine.ChangeState(new CharacterIdleState(Controller));
            return;
        }

        // 공격 거리 체크 및 이동
        _positionDelta = _target.transform.position - Controller.transform.position;
        if (_positionDelta.magnitude > Controller.Data.AttackReach)
        {
            if (_lastAttackTime < Time.time + _attackInterval)
                Attack();
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float tan = Mathf.Atan2(_positionDelta.z, _positionDelta.x);
        Quaternion rotation = Quaternion.Euler(0, Mathf.Rad2Deg * tan, 0);

        Controller.transform.SetPositionAndRotation(
            Controller.transform.position + _positionDelta * (Time.deltaTime * _moveSpeed), rotation);
    }

    private void Attack()
    {
        _lastAttackTime = Time.time;
        Animator.SetTrigger(CharacterAnimHash.AttackTrigger);
        _target.TakeDamage(Controller.Data.Attack);
    }
}