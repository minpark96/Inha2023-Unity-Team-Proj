using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Actor;

public class CmdMove : PlayerCommand
{


    Vector3 _moveDir;

    public CmdMove(Actor actor)
    {
        bodyHandler = actor.BodyHandler;
        this.actor = actor;
       
    }
    public override void Execute(Vector3 moveDir = default)
    {
        _moveDir = moveDir;



        RunCyclePoseBody();
        RunCyclePoseArm(Define.Side.Left, leftArmPose);
        RunCyclePoseArm(Define.Side.Right, rightArmPose);
        RunCyclePoseLeg(Define.Side.Left, leftLegPose);
        RunCyclePoseLeg(Define.Side.Right, rightLegPose);
    }



    private void RunCyclePoseLeg(Define.Side side, Define.BodyPose pose)
    {
        Transform hip = bodyHandler.Hip.transform;
        Transform thighTrans = null;
        Transform legTrans = null;

        Rigidbody thighRigid = null;
        Rigidbody legRigid = null;

        switch (side)
        {
            case Define.Side.Left:
                thighTrans = bodyHandler.LeftThigh.transform;
                legTrans = bodyHandler.LeftLeg.transform;

                thighRigid = bodyHandler.LeftThigh.GetComponent<Rigidbody>();
                legRigid = bodyHandler.LeftLeg.PartRigidbody;
                break;
            case Define.Side.Right:
                thighTrans = bodyHandler.RightThigh.transform;
                legTrans = bodyHandler.RightLeg.transform;
                thighRigid = bodyHandler.RightThigh.PartRigidbody;
                legRigid = bodyHandler.RightLeg.PartRigidbody;
                break;
        }

        switch (pose)
        {
            case Define.BodyPose.Bent:
                AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 2f * applyedForce);
                AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 2f * applyedForce);
                break;
            case Define.BodyPose.Forward:
                AlignToVector(thighRigid, -thighTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * applyedForce);
                AlignToVector(legRigid, -legTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * applyedForce);
                thighRigid.AddForce(-_moveDir / 2f, ForceMode.VelocityChange);
                legRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);

                break;
            case Define.BodyPose.Straight:
                AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * applyedForce);
                AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * applyedForce);
                thighRigid.AddForce(hip.up * 2f * applyedForce);
                legRigid.AddForce(-hip.up * 2f * applyedForce);
                legRigid.AddForce(-runVectorForce2, ForceMode.VelocityChange);

                break;
            case Define.BodyPose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * applyedForce);
                break;
        }
    }

    private void RunCyclePoseArm(Define.Side side, Define.BodyPose pose)
    {
        Vector3 vector = Vector3.zero;
        Transform partTransform = bodyHandler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody rigidbody = null;
        Rigidbody rigidbody2 = null;
        Rigidbody rigidbody3 = null;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;
        switch (side)
        {
            case Define.Side.Left:
                transform = bodyHandler.LeftArm.transform;
                transform2 = bodyHandler.LeftForeArm.transform;
                rigidbody = bodyHandler.LeftArm.PartRigidbody;
                rigidbody2 = bodyHandler.LeftForeArm.PartRigidbody;
                rigidbody3 = bodyHandler.LeftHand.PartRigidbody;
                vector = bodyHandler.Chest.transform.right;
                break;
            case Define.Side.Right:
                transform = bodyHandler.RightArm.transform;
                transform2 = bodyHandler.RightForeArm.transform;
                rigidbody = bodyHandler.RightArm.PartRigidbody;
                rigidbody2 = bodyHandler.RightForeArm.PartRigidbody;
                rigidbody3 = bodyHandler.RightHand.PartRigidbody;
                vector = -bodyHandler.Chest.transform.right;
                break;
        }
        if ((actor.flags & ActorFlag.Run) != ActorFlag.Run)
        {
            switch (pose)
            {
                case Define.BodyPose.Bent:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -_moveDir / 4f, 0.1f, 4f * applyedForce * armForceCoef);
                    break;
                case Define.BodyPose.Forward:
                    AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, _moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * applyedForce * armForceCoef);
                    break;
                case Define.BodyPose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
                    break;
                case Define.BodyPose.Behind:
                    AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
                    break;
            }
            return;
        }
        switch (pose)
        {
            case Define.BodyPose.Bent:
                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, -_moveDir, 0.1f, 4f * applyedForce * armForceCoef);
                rigidbody.AddForce(-_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Define.BodyPose.Forward:
                AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * applyedForce);
                AlignToVector(rigidbody2, transform2.forward, _moveDir + -partTransform.forward + -vector, 0.1f, 4f * applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Define.BodyPose.Straight:
                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
                rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Define.BodyPose.Behind:
                AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
        }
    }


    private void RunCyclePoseBody()
    {
        bodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        bodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        AlignToVector(bodyHandler.Chest.PartRigidbody, -bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(bodyHandler.Chest.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, -bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, -bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

        if ((actor.flags & ActorFlag.Run) == ActorFlag.Run)
        {
            bodyHandler.Hip.PartRigidbody.AddForce(_moveDir.normalized * RunSpeed * runSpeedOffset * Time.deltaTime * 1.35f);
            if (bodyHandler.Hip.PartRigidbody.velocity.magnitude > MaxSpeed)
                bodyHandler.Hip.PartRigidbody.velocity = bodyHandler.Hip.PartRigidbody.velocity.normalized * MaxSpeed * 1.15f;
        }
        else
        {
            bodyHandler.Hip.PartRigidbody.AddForce(_moveDir.normalized * RunSpeed * runSpeedOffset * Time.deltaTime);
            if (bodyHandler.Hip.PartRigidbody.velocity.magnitude > MaxSpeed)
                bodyHandler.Hip.PartRigidbody.velocity = bodyHandler.Hip.PartRigidbody.velocity.normalized * MaxSpeed;
        }

    }

}
