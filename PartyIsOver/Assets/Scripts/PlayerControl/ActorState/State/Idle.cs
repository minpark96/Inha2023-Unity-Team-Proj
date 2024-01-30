using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Idle : Grounded
{
    private float horizontalInput;
    private float verticalInput;
    private float idleTimer = 0;

    public Idle(MovementSM stateMachine) : base("Idel", stateMachine) {}
    public override void Enter()
    {
        base.Enter();
        //TODO : 속도 0으로 설정
        horizontalInput = 0f;
        verticalInput = 0f;
        idleTimer = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //TODO : 키 입력이 생기면 Moving으로 변경
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon || Mathf.Abs(verticalInput) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).MovingState);

    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        idleTimer = Mathf.Clamp(idleTimer + Time.fixedDeltaTime, -60f, 30f);

        if (sm.Rigidbody.velocity.magnitude > 1f)
            sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * sm.Rigidbody.velocity.magnitude * 0.6f;
    }

}