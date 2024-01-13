using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSM : StateMachine
{
    public Idle idleState;
    public Moving movingState;

    public Rigidbody rigidbody;
    //speed는 ScriptableObject 로 변경해서 받아야함
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
