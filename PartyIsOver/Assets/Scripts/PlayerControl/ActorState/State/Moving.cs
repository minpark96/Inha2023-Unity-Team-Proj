using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Moving : BaseState
{
    private MovementSM _sm;
    private MovementSM msm;
    private float inputspeed;
    private Vector3 MoveInput;
    private float _runSpeedOffset = 350f;
    float RunSpeed = 1.5f;
    float MaxSpeed = 2f;
    Vector3 _moveDir;

    //GetComponent�� ���ϴϱ� public���� �ؾ��� ��

    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) 
    {
        _sm = (MovementSM)stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        //TODO : �ӵ� 0���� ����
        inputspeed = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : Ű �Է� �߰�
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        MoveInput = new Vector3(horizontalInput, 0, verticalInput);

        if (MoveInput != Vector3.zero)
        {
            // Ű �Է��� �����Ǹ� Moving ���� ����
            inputspeed = 1f;
        }
        else
        {
            // Ű �Է��� ������ Idle ���·� ��ȯ
            stateMachine.ChangeState(_sm.idleState);
        }
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        Vector3 lookForward = new Vector3(msm.Camera.forward.x, 0f, msm.Camera.forward.z).normalized;
        Vector3 lookRight = new Vector3(msm.Camera.right.x, 0f, msm.Camera.right.z).normalized;
        _moveDir = lookForward * MoveInput.z + lookRight * MoveInput.x;

        Vector3 vel = MoveInput * _sm.speed;
        _sm.rigidbody.velocity = vel;

        _sm.rigidbody.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime);
        if (_sm.rigidbody.velocity.magnitude > MaxSpeed)
            _sm.rigidbody.velocity = _sm.rigidbody.velocity.normalized * MaxSpeed;
    }
}
