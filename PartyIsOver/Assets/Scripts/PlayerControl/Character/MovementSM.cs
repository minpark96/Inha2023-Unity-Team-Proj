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

    public Transform Camera;
    public CameraControl CameraControl;

    private void Awake()
    {
        idleState = new Idle(this, CameraControl);
        movingState = new Moving(this);

        Init();
    }

    private void Start()
    {
        Camera = CameraControl.CameraArm;
    }

    private void Init()
    {
        if(CameraControl !=null)
            CameraControl =GetComponent<CameraControl>();
    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }
}
