using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node
{
    private Animator _animator;

    private Transform _lastTarget;
    //이친구도 바꿔야함
    private PlayerController playerController;
    StatusHandler playerstatu;


    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    public TaskAttack(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if(target != _lastTarget)
        {
            //어떤 함수를 불러오면 바로 기절시키는 함수를 불러오는 스크립트로 바꿔야함
            playerController = target.GetComponent<PlayerController>();
            _lastTarget = target;
        }
        
        _attackCounter += Time.deltaTime;
        if(_attackCounter >= _attackTime)
        {
            playerstatu = playerController.GetComponent<StatusHandler>();
            bool playerIsDead = playerstatu.HasFreeze;
            // hip 하는 스크립트 추가

            //bool playerIsDead 가 참이면은
            if(true)
            {
                ClearData("target");
                _animator.SetBool("Attacking", false);
                _animator.SetBool("Walking", true);
            }
            else
            {
                _attackCounter = 0f;
            }
        }

        state = NodeState.Running;
        return state;
    }

}
