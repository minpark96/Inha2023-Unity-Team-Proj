using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBomb : MonoBehaviour
{
    public float SensingRange;
    private Transform _targetPlayer;
    private NavMeshAgent _nav;
    private Animator _animator;

    float _hp;
    SphereCollider SphereCollider;

    

    Define.State state;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();

        SphereCollider = GetComponent<SphereCollider>();
        SphereCollider.radius = SensingRange;
        _hp = 10;
        state = Define.State.Idle;
        StartCoroutine(StateMachine());
    }
    
    IEnumerator StateMachine()
    {
        while (_hp > 0)
            yield return state.ToString();
    }

    IEnumerator IDLE()
    {

        yield return null;
    }


}
