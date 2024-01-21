using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static AniFrameData;

public class Jumping2 : BaseState
{
    private MovementSM sm;
    private float horizontalInput;
    private float verticalInput;
    private float maxSpeed = 2f;

    private Vector3 moveInput;
    private Vector3 _moveDir;

    public Jumping2(string name, MovementSM stateMachine) : base(name, stateMachine)
    {
        sm = (MovementSM)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("üũ Ȯ��");

        sm.Rigidbody.AddForce(Vector3.up * sm.Speed * 0.5f);

    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        moveInput = new Vector3(horizontalInput, 0, verticalInput);

        //TODO : ���⼭ üũ�ؼ� ��ȯ �ϴ°� ���� ��
        if (IsGrounded())
            stateMachine.ChangeState(sm.IdleState);
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        //�ٶ󺸴� �������� ���� �� ���� ���߿� ȸ���� �����ϵ��� �ϱ� ���ؼ�
        Vector3 lookForward = new Vector3(stateMachine.PlayerCharacter.CameraTransform.forward.x, 0f, stateMachine.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(stateMachine.PlayerCharacter.CameraTransform.right.x, 0f, stateMachine.PlayerCharacter.CameraTransform.right.z).normalized;
        _moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        sm.Rigidbody.AddForce(_moveDir.normalized * sm.Speed * Time.deltaTime * 0.5f);
        if (sm.Rigidbody.velocity.magnitude > maxSpeed)
            sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * maxSpeed;
    }

    private bool IsGrounded()
    {
        float raycastDistance = 0.1f;
        RaycastHit hit;
        if (Physics.Raycast(sm.FootRigidbody.position, Vector3.down, out hit, raycastDistance))
        {
            sm.IsGround = false;
            return true;
        }

        return false;
    }

}
