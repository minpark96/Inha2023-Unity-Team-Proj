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
    [HideInInspector]
    public Jumping JumpingState;


    public Rigidbody Rigidbody;
    public Rigidbody FootRigidbody;
    //speed는 ScriptableObject 로 변경해서 받아야함
    public float Speed = 4;
    public float RunSpeed = 1.35f;

    private void Awake()
    {
        IdleState = new Idle(this);
        MovingState = new Moving(this);
        JumpingState = new Jumping(this);

        Init();
    }

    private void Init()
    {
        Transform hip = transform.Find("GreenHip");
        Rigidbody = hip.GetComponent<Rigidbody>();
        Transform foot = transform.Find("foot_l");
        FootRigidbody = foot.GetComponent<Rigidbody>();
        
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }
}
