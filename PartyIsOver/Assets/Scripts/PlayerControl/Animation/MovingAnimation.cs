using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class MovingAnimation : Moving
{
    public MovingAnimation(MovementSM stateMachine) : base(stateMachine) { }

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

    public void RunCycleUpdate()
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

    public void RunCyclePoseLeg(Define.Side side, Define.Pose pose)
    {

        Transform hip = sm.BodyHandler.Hip.transform;
        Transform thighTrans = sm.BodyHandler.LeftThigh.transform;
        Transform legTrans = sm.BodyHandler.LeftLeg.transform;

        Rigidbody thighRigid = sm.BodyHandler.LeftThigh.GetComponent<Rigidbody>();
        Rigidbody legRigid = sm.BodyHandler.LeftLeg.PartRigidbody;

        if (side == Define.Side.Right)
        {
            thighTrans = sm.BodyHandler.RightThigh.transform;
            legTrans = sm.BodyHandler.RightLeg.transform;
            thighRigid = sm.BodyHandler.RightThigh.PartRigidbody;
            legRigid = sm.BodyHandler.RightLeg.PartRigidbody;
        }

        switch (pose)
        {
            case Define.Pose.Bent:
                AlignToVector(thighRigid, -thighTrans.forward, moveDir, 0.1f, 2f * applyedForce);
                AlignToVector(legRigid, legTrans.forward, moveDir, 0.1f, 2f * applyedForce);
                break;
            case Define.Pose.Forward:
                AlignToVector(thighRigid, -thighTrans.forward, moveDir + -hip.up / 2f, 0.1f, 4f * applyedForce);
                AlignToVector(legRigid, -legTrans.forward, moveDir + -hip.up / 2f, 0.1f, 4f * applyedForce);
                thighRigid.AddForce(-moveDir / 2f, ForceMode.VelocityChange);
                legRigid.AddForce(moveDir / 2f, ForceMode.VelocityChange);
                break;
            case Define.Pose.Straight:
                AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * applyedForce);
                AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * applyedForce);
                thighRigid.AddForce(hip.up * 2f * applyedForce);
                legRigid.AddForce(-hip.up * 2f * applyedForce);
                legRigid.AddForce(-runVectorForce2, ForceMode.VelocityChange);
                break;
            case Define.Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, moveDir * 2f, 0.1f, 2f * applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -moveDir * 2f, 0.1f, 2f * applyedForce);
                break;
        }

    }

    public void RunCyclePoseArm(Define.Side side, Define.Pose pose)
    {
        Vector3 vector = sm.BodyHandler.Chest.transform.right;
        Transform partTransform = sm.BodyHandler.Chest.transform;
        Transform transform = sm.BodyHandler.LeftArm.transform;
        Transform transform2 = sm.BodyHandler.LeftForearm.transform;
        Rigidbody rigidbody = sm.BodyHandler.LeftArm.PartRigidbody;
        Rigidbody rigidbody2 = sm.BodyHandler.LeftForearm.PartRigidbody;
        Rigidbody rigidbody3 = sm.BodyHandler.LeftHand.PartRigidbody;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;
        if (side == Define.Side.Right)
        {
            transform = sm.BodyHandler.RightArm.transform;
            transform2 = sm.BodyHandler.RightForearm.transform;
            rigidbody = sm.BodyHandler.RightArm.PartRigidbody;
            rigidbody2 = sm.BodyHandler.RightForearm.PartRigidbody;
            rigidbody3 = sm.BodyHandler.RightHand.PartRigidbody;
            vector = -sm.BodyHandler.Chest.transform.right;
        }

        if (!isRun)
        {
            switch (pose)
            {
                case Define.Pose.Bent:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -moveDir / 4f, 0.1f, 4f * applyedForce * armForceCoef);
                    rigidbody.AddForce(-moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Forward:
                    AlignToVector(rigidbody, transform.forward, moveDir + -vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);

                    rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Behind:
                    AlignToVector(rigidbody, transform.forward, moveDir, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
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
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -moveDir, 0.1f, 4f * applyedForce * armForceCoef);
                    rigidbody.AddForce(-moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(moveDir * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Forward:
                    AlignToVector(rigidbody, transform.forward, moveDir + -vector, 0.1f, 4f * applyedForce);
                    AlignToVector(rigidbody2, transform2.forward, moveDir + -partTransform.forward + -vector, 0.1f, 4f * applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
                    rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
                case Define.Pose.Behind:
                    AlignToVector(rigidbody, transform.forward, moveDir, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
                    rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                    break;
            }
        }
    }

    public void RunCyclePoseBody()
    {
        Vector3 lookForward = new Vector3(sm.PlayerCharacter.CameraTransform.forward.x, 0f, sm.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(sm.PlayerCharacter.CameraTransform.right.x, 0f, sm.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        sm.BodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + moveDir), ForceMode.VelocityChange);
        sm.BodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, -sm.BodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, -sm.BodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, -sm.BodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, sm.BodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

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
