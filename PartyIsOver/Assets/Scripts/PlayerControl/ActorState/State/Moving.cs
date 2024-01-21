using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class Moving : Grounded
{
    private float inputspeed;
    private float maxSpeed = 2f;
    private float horizontalInput;
    private float verticalInput;

    private bool isRun;

    private Vector3 moveInput;
    private Vector3 moveDir;

    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) {}
    public override void Enter()
    {
        base.Enter();
        inputspeed = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        moveInput = new Vector3(horizontalInput, 0, verticalInput);

        if(Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }

        if (moveInput != Vector3.zero)
        {
            inputspeed = 1f;
        }
        else
        {
            stateMachine.ChangeState(sm.IdleState);
        }
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        Vector3 lookForward = new Vector3(stateMachine.PlayerCharacter.CameraTransform.forward.x, 0f, stateMachine.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(stateMachine.PlayerCharacter.CameraTransform.right.x, 0f, stateMachine.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        if(isRun)
        {
            sm.Rigidbody.AddForce(moveDir.normalized * sm.Speed * Time.deltaTime * sm.RunSpeed);
            if (sm.Rigidbody.velocity.magnitude > maxSpeed)
                sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * maxSpeed * 1.15f;
        }
        else
        {
            sm.Rigidbody.AddForce(moveDir.normalized * sm.Speed * Time.deltaTime);
            if (sm.Rigidbody.velocity.magnitude > maxSpeed)
                sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * maxSpeed;
        }
    }
}
