using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckEnemyInAttackRange : Node
{
    private static int _enemyLayerMask = (1 << 26) | (1 << 27) | (1 << 28) | (1 << 29) | (1 << 30) | (1 << 31);

    private Transform _transform;
    private Animator _animator;

    public CheckEnemyInAttackRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if(t != null)
        {
            state = NodeState.Failure;
            return state;
        }

        Transform target = (Transform)t;
        if(Vector3.Distance(_transform.position, target.position) <= GuardBT.attackRange)
        {
            _animator.SetBool("Attacking", true);
            _animator.SetBool("Walking", false);

            state = NodeState.Success;
            return state;
        }
        state = NodeState.Failure;
        return state;
    }
}
