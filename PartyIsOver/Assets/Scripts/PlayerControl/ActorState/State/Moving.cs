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
    private float MovingMotionSpeed;
    private float MovingMotionTimer = 0;
    private float _applyedForce = 800f;

    private bool isRun;

    private Vector3 _runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);
    private Vector3 moveInput;
    private Vector3 moveDir;

    Define.Pose leftLegPose;
    Define.Pose rightLegPose;
    Define.Pose leftArmPose;
    Define.Pose rightArmPose;

    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) {}
    public override void Enter()
    {
        base.Enter();
        inputspeed = 0f;
        if (Random.Range(0, 2) == 1)
        {

            leftLegPose = Define.Pose.Bent;
            rightLegPose = Define.Pose.Straight;
            leftArmPose = Define.Pose.Straight;
            rightArmPose = Define.Pose.Bent;
        }
        else
        {
            leftLegPose = Define.Pose.Straight;
            rightLegPose = Define.Pose.Bent;
            leftArmPose = Define.Pose.Bent;
            rightArmPose = Define.Pose.Straight;
        }
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
            MovingMotionSpeed = 0.15f;
        }
        else
        {
            isRun = false;
            MovingMotionSpeed = 0.1f;
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

        RunCycleUpdate();
        RunCyclePoseBody();
        RunCyclePoseArm(Define.Side.Left, leftArmPose);
        RunCyclePoseArm(Define.Side.Right, rightArmPose);
        RunCyclePoseLeg(Define.Side.Left, leftLegPose);
        RunCyclePoseLeg(Define.Side.Right, rightLegPose);
    }

    private void RunCycleUpdate()
    {
        if (MovingMotionTimer < MovingMotionSpeed)
        {
            MovingMotionTimer += Time.fixedDeltaTime;
            return;
        }
        MovingMotionTimer = 0f;
        int num = (int)leftArmPose;
        num++;
        leftArmPose = ((num <= 3) ? ((Define.Pose)num) : Define.Pose.Bent);
        int num2 = (int)rightArmPose;
        num2++;
        rightArmPose = ((num2 <= 3) ? ((Define.Pose)num2) : Define.Pose.Bent);
        int num3 = (int)leftLegPose;
        num3++;
        leftLegPose = ((num3 <= 3) ? ((Define.Pose)num3) : Define.Pose.Bent);
        int num4 = (int)rightLegPose;
        num4++;
        rightLegPose = ((num4 <= 3) ? ((Define.Pose)num4) : Define.Pose.Bent);
    }

    private void RunCyclePoseLeg(Define.Side side, Define.Pose pose)
    {

        Transform hip = sm.PlayerCharacter.bodyHandler.Hip.transform;
        Transform thighTrans = sm.PlayerCharacter.bodyHandler.LeftThigh.transform;
        Transform legTrans = sm.PlayerCharacter.bodyHandler.LeftLeg.transform;

        Rigidbody thighRigid = sm.PlayerCharacter.bodyHandler.LeftThigh.GetComponent<Rigidbody>();
        Rigidbody legRigid = sm.PlayerCharacter.bodyHandler.LeftLeg.PartRigidbody;

        if (side == Define.Side.Right)
        {
            thighTrans = sm.PlayerCharacter.bodyHandler.RightThigh.transform;
            legTrans = sm.PlayerCharacter.bodyHandler.RightLeg.transform;
            thighRigid = sm.PlayerCharacter.bodyHandler.RightThigh.PartRigidbody;
            legRigid = sm.PlayerCharacter.bodyHandler.RightLeg.PartRigidbody;
        }

        switch (pose)
        {
            case Define.Pose.Bent:
                AlignToVector(thighRigid, -thighTrans.forward, moveDir, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, moveDir, 0.1f, 2f * _applyedForce);
                break;
            case Define.Pose.Forward:
                AlignToVector(thighRigid, -thighTrans.forward, moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                thighRigid.AddForce(-moveDir / 2f, ForceMode.VelocityChange);
                legRigid.AddForce(moveDir / 2f, ForceMode.VelocityChange);
                break;
            case Define.Pose.Straight:
                AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                thighRigid.AddForce(hip.up * 2f * _applyedForce);
                legRigid.AddForce(-hip.up * 2f * _applyedForce);
                legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                break;
            case Define.Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, moveDir * 2f, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -moveDir * 2f, 0.1f, 2f * _applyedForce);
                break;
        }

    }

    private void RunCyclePoseArm(Define.Side side, Define.Pose pose)
    {
        Vector3 vector = sm.PlayerCharacter.bodyHandler.Chest.transform.right;
        Transform partTransform = sm.PlayerCharacter.bodyHandler.Chest.transform;
        Transform transform = sm.PlayerCharacter.bodyHandler.LeftArm.transform;
        Transform transform2 = sm.PlayerCharacter.bodyHandler.LeftForearm.transform;
        Rigidbody rigidbody = sm.PlayerCharacter.bodyHandler.LeftArm.PartRigidbody;
        Rigidbody rigidbody2 = sm.PlayerCharacter.bodyHandler.LeftForearm.PartRigidbody;
        Rigidbody rigidbody3 = sm.PlayerCharacter.bodyHandler.LeftHand.PartRigidbody;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;
        if (side == Define.Side.Right)
        {
            transform = sm.PlayerCharacter.bodyHandler.RightArm.transform;
            transform2 = sm.PlayerCharacter.bodyHandler.RightForearm.transform;
            rigidbody = sm.PlayerCharacter.bodyHandler.RightArm.PartRigidbody;
            rigidbody2 = sm.PlayerCharacter.bodyHandler.RightForearm.PartRigidbody;
            rigidbody3 = sm.PlayerCharacter.bodyHandler.RightHand.PartRigidbody;
            vector = -sm.PlayerCharacter.bodyHandler.Chest.transform.right;
        }

        if (!isRun)
        {
            switch (pose)
            {
                case Define.Pose.Bent:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -moveDir / 4f, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(-moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Forward:
                    AlignToVector(rigidbody, transform.forward, moveDir + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);

                    rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Behind:
                    AlignToVector(rigidbody, transform.forward, moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
            }
        }
        else
        {
            switch (pose)
            {
                case Define.Pose.Bent:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(-moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Forward:
                    AlignToVector(rigidbody, transform.forward, moveDir + -vector, 0.1f, 4f * _applyedForce);
                    AlignToVector(rigidbody2, transform2.forward, moveDir + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Behind:
                    AlignToVector(rigidbody, transform.forward, moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
            }
        }
    }

    private void RunCyclePoseBody()
    {
        Vector3 lookForward = new Vector3(stateMachine.PlayerCharacter.CameraTransform.forward.x, 0f, stateMachine.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(stateMachine.PlayerCharacter.CameraTransform.right.x, 0f, stateMachine.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + moveDir), ForceMode.VelocityChange);
        sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * _applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, sm.PlayerCharacter.bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

        if (isRun)
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