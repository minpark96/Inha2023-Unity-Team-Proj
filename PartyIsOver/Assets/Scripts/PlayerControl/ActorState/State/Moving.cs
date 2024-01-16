using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class Moving : BaseState
{
    private MovementSM _sm;
    //private PlayerCharacter _playerCharacter;
    private PlayerCharacter PlayerCharacter;

    private float _inputspeed;
    private float _runSpeedOffset = 350f;
    private float _runSpeed = 1.5f;
    private float _maxSpeed = 2f;
    private Vector3 _moveInput;
    private Vector3 _moveDir;

    //GetComponent�� ���ϴϱ� public���� �ؾ��� ��

    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) 
    {
        _sm = (MovementSM)stateMachine;
        
    }
    public override void Enter()
    {
        base.Enter();
        //TODO : �ӵ� 0���� ����
        _inputspeed = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : Ű �Է� �߰�
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        _moveInput = new Vector3(horizontalInput, 0, verticalInput);

        if (_moveInput != Vector3.zero)
        {
            // Ű �Է��� �����Ǹ� Moving ���� ����
            _inputspeed = 1f;
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

        Vector3 lookForward = new Vector3(PlayerCharacter.CameraTransform.forward.x, 0f, PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(PlayerCharacter.CameraTransform.right.x, 0f, PlayerCharacter.CameraTransform.right.z).normalized;
        _moveDir = lookForward * _moveInput.z + lookRight * _moveInput.x;

        Vector3 vel = _moveInput * _sm.speed;
        _sm.rigidbody.velocity = vel;

        _sm.rigidbody.AddForce(_moveDir.normalized * _runSpeed * _runSpeedOffset * Time.deltaTime);
        if (_sm.rigidbody.velocity.magnitude > _maxSpeed)
            _sm.rigidbody.velocity = _sm.rigidbody.velocity.normalized * _maxSpeed;
    }
}
