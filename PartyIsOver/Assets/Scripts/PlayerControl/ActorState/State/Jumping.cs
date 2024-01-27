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
    private CharacterPhysicsMotion[] JumpingAnimation;

    public Jumping(MovementSM stateMachine) : base("Jumping", stateMachine)
    {
        sm = (MovementSM)stateMachine;
        groundLayer = LayerMask.GetMask("ClimbObject");
    }

    public override void Enter()
    {
        base.Enter();

        sm.Rigidbody.AddForce(Vector3.up * sm.Speed * 0.5f);


        // TODO : 애니메이션 추가 작업
        //AnimateWithDirectedForce(MoveForceJumpAniData, i, Vector3.up);
        /*        for(int i =0; i< JumpingAnimation.Length; i++)
                {
                    int _elementCount = 1;
                    Vector3 direction = GetForceDirection(JumpingAnimation[_elementCount], i);
                    JumpingAnimation[_elementCount].ActionRigidbodies[i].AddForce(direction * JumpingAnimation[_elementCount].ActionForceValues[i], ForceMode.Impulse);
                }*/

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

        grounded = sm.FootRigidbody.velocity.y < Mathf.Epsilon && IsGrounded();
        Debug.Log(sm.FootRigidbody.velocity.y + " " + IsGrounded());

        Vector3 lookForward = new Vector3(sm.PlayerCharacter.CameraTransform.forward.x, 0f, sm.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(sm.PlayerCharacter.CameraTransform.right.x, 0f, sm.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + moveDir), ForceMode.VelocityChange);
        sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, sm.PlayerCharacter.bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

        if (sm.Rigidbody.velocity.magnitude > maxSpeed)
            sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * maxSpeed;
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 0.05f; 

        //충돌이 되었는지 확인 확인후 참이면 Idle 상태로 전환
        if (Physics.Raycast(sm.FootRigidbody.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            return true; 
        }
        return false;
    }
}