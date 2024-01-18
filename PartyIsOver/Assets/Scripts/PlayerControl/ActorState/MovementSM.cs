using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class MovementSM : StateMachine
{
    [HideInInspector]
    public Idle IdleState;
    [HideInInspector]
    public Moving MovingState;

    public Rigidbody Rigidbody;
    //speed�� ScriptableObject �� �����ؼ� �޾ƾ���
    public float Speed = 4;
    public float RunSpeed = 1.35f;

    private void Awake()
    {
        IdleState = new Idle(this);
        MovingState = new Moving(this);

        Init();
    }

    private void Init()
    {
        Transform Hip = transform.Find("GreenHip");
        Rigidbody = Hip.GetComponent<Rigidbody>();
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }
}
