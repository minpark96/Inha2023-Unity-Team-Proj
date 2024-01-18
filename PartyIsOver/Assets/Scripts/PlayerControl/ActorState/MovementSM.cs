using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class MovementSM : StateMachine
{
    [HideInInspector]
    public Idle idleState;
    [HideInInspector]
    public Moving movingState;

    public Rigidbody rigidbody;
    //speed�� ScriptableObject �� �����ؼ� �޾ƾ���
    public float speed = 4;

    private void Awake()
    {
        idleState = new Idle(this);
        movingState = new Moving(this);

        Init();
    }

    private void Init()
    {
        Transform Hip = transform.Find("GreenHip");
        rigidbody = Hip.GetComponent<Rigidbody>();
    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }
}
