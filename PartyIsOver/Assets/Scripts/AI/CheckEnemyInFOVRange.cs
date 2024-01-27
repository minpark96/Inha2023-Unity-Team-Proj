using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckEnemyInFOVRange : Node
{
    //Define으로 변환
    private static int _enemyLayerMask = (1 << 26) | (1 << 27) | (1 << 28) | (1 << 29) | (1 << 30) | (1 << 31) ;
    private Transform _transform;
    private Animator _animator;

    public CheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if(t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position, GuardBT.fovRange, _enemyLayerMask);
            if(colliders.Length > 0 )
            {
                parent.parent.SetData("target", colliders[0].transform);
                _animator.SetBool("Walking", true);
                state = NodeState.Success;
                return state;
            }
            state = NodeState.Failure;
            return state;
        }
        else
        {
            Transform targetTransform = (Transform)t;
            float distanceToTarget = Vector3.Distance(_transform.position, targetTransform.position);

            if(distanceToTarget > GuardBT.fovRange * 1.5f)
            {
                parent.parent.SetData("target", null);
                _animator.SetBool("Walking", false);
            }
        }
        state = NodeState.Success;
        return state;
    }

}
