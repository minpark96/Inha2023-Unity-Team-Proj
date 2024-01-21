using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Jumping : BaseState
{
    protected MovementSM sm;
    private bool grounded;

    private LayerMask groundLayer;

    public Jumping(MovementSM stateMachine) : base("Jumping", stateMachine)
    {
        sm = (MovementSM)stateMachine;
        groundLayer = LayerMask.GetMask("Ground");
    }

    public override void Enter()
    {
        base.Enter();
        //이단 점프 방지
        if(IsGrounded())
            sm.Rigidbody.AddForce(Vector3.up * sm.Speed * 0.5f);

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (grounded)
            stateMachine.ChangeState(sm.IdleState);
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        grounded = /*Mathf.Abs(sm.FootRigidbody.velocity.y) < 0.5f &&*/ IsGrounded();

    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 0.05f; 

        if (Physics.Raycast(sm.FootRigidbody.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            return true;
        }
        return false;
    }

}
