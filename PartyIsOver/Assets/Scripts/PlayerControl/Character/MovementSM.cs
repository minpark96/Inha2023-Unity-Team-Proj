using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSM : StateMachine
{
    public Idle idleState;
    public Moving movingState;

    public Rigidbody rigidbody;
    //speed�� ScriptableObject �� �����ؼ� �޾ƾ���
    public float speed = 4;

    private void Awake()
    {
        Debug.Log("Awake");
        idleState = new Idle(this);
        movingState = new Moving(this);

        Init();
    }

    private void Start()
    {

    }

    private void Init()
    {

    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }
}
