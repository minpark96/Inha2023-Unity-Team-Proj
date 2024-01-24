using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class MovingAnimation : MonoBehaviour
{
    //public void Move()
    //{
    //    if (MoveInput.magnitude == 0f)
    //        _actor.actorState = Actor.ActorState.Stand;

    //    if (_actor.actorState == ActorState.Run)
    //    {
    //        _cycleSpeed = 0.1f;
    //    }
    //    else
    //    {
    //        _cycleSpeed = 0.15f;
    //    }
    //    if (isStateChange)
    //    {
    //        if (Random.Range(0, 2) == 1)
    //        {
    //            leftLegPose = Pose.Bent;
    //            rightLegPose = Pose.Straight;
    //            leftArmPose = Pose.Straight;
    //            rightArmPose = Pose.Bent;
    //        }
    //        else
    //        {
    //            leftLegPose = Pose.Straight;
    //            rightLegPose = Pose.Bent;
    //            leftArmPose = Pose.Bent;
    //            rightArmPose = Pose.Straight;
    //        }
    //    }
    //    //Stand();
    //    RunCycleUpdate();
    //    RunCyclePoseBody();
    //    RunCyclePoseArm(Side.Left, leftArmPose);
    //    RunCyclePoseArm(Side.Right, rightArmPose);
    //    RunCyclePoseLeg(Side.Left, leftLegPose);
    //    RunCyclePoseLeg(Side.Right, rightLegPose);
    //}

    //private void RunCycleUpdate()
    //{
    //    if (_cycleTimer < _cycleSpeed)
    //    {
    //        _cycleTimer += Time.deltaTime;
    //        return;
    //    }
    //    _cycleTimer = 0f;
    //    int num = (int)leftArmPose;
    //    num++;
    //    leftArmPose = ((num <= 3) ? ((Pose)num) : Pose.Bent);
    //    int num2 = (int)rightArmPose;
    //    num2++;
    //    rightArmPose = ((num2 <= 3) ? ((Pose)num2) : Pose.Bent);
    //    int num3 = (int)leftLegPose;
    //    num3++;
    //    leftLegPose = ((num3 <= 3) ? ((Pose)num3) : Pose.Bent);
    //    int num4 = (int)rightLegPose;
    //    num4++;
    //    rightLegPose = ((num4 <= 3) ? ((Pose)num4) : Pose.Bent);
    //}

    //private void RunCyclePoseLeg(Side side, Pose pose)
    //{
    //    Transform hip = _bodyHandler.Hip.transform;
    //    Transform thighTrans = null;
    //    Transform legTrans = null;

    //    Rigidbody thighRigid = null;
    //    Rigidbody legRigid = null;
    //    Rigidbody footRigid = null;

    //    switch (side)
    //    {
    //        case Side.Left:
    //            thighTrans = _bodyHandler.LeftThigh.transform;
    //            legTrans = _bodyHandler.LeftLeg.transform;

    //            thighRigid = _bodyHandler.LeftThigh.GetComponent<Rigidbody>();
    //            legRigid = _bodyHandler.LeftLeg.PartRigidbody;
    //            footRigid = _bodyHandler.LeftFoot.PartRigidbody;
    //            break;
    //        case Side.Right:
    //            thighTrans = _bodyHandler.RightThigh.transform;
    //            legTrans = _bodyHandler.RightLeg.transform;
    //            thighRigid = _bodyHandler.RightThigh.PartRigidbody;
    //            legRigid = _bodyHandler.RightLeg.PartRigidbody;
    //            footRigid = _bodyHandler.RightFoot.PartRigidbody;
    //            break;
    //    }

    //    switch (pose)
    //    {
    //        case Pose.Bent:
    //            AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
    //            AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
    //            break;
    //        case Pose.Forward:
    //            AlignToVector(thighRigid, -thighTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
    //            AlignToVector(legRigid, -legTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
    //            if (!isDuck)
    //            {
    //                thighRigid.AddForce(-_moveDir / 2f, ForceMode.VelocityChange);
    //                legRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);
    //            }
    //            break;
    //        case Pose.Straight:
    //            AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
    //            AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
    //            if (!isDuck)
    //            {
    //                thighRigid.AddForce(hip.up * 2f * _applyedForce);
    //                legRigid.AddForce(-hip.up * 2f * _applyedForce);
    //                legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
    //            }
    //            break;
    //        case Pose.Behind:
    //            AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
    //            AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
    //            if (isDuck)
    //            {
    //                _bodyHandler.Hip.PartRigidbody.AddForce(_runVectorForce2, ForceMode.VelocityChange);
    //                _bodyHandler.Ball.PartRigidbody.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
    //                legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
    //            }
    //            break;
    //    }
    //}

    //private void RunCyclePoseArm(Side side, Pose pose)
    //{
    //    Vector3 vector = Vector3.zero;
    //    Transform partTransform = _bodyHandler.Chest.transform;
    //    Transform transform = null;
    //    Transform transform2 = null;
    //    Rigidbody rigidbody = null;
    //    Rigidbody rigidbody2 = null;
    //    Rigidbody rigidbody3 = null;

    //    float armForceCoef = 0.3f;
    //    float armForceRunCoef = 0.6f;
    //    switch (side)
    //    {
    //        case Side.Left:
    //            transform = _bodyHandler.LeftArm.transform;
    //            transform2 = _bodyHandler.LeftForearm.transform;
    //            rigidbody = _bodyHandler.LeftArm.PartRigidbody;
    //            rigidbody2 = _bodyHandler.LeftForearm.PartRigidbody;
    //            rigidbody3 = _bodyHandler.LeftHand.PartRigidbody;
    //            vector = _bodyHandler.Chest.transform.right;
    //            break;
    //        case Side.Right:
    //            transform = _bodyHandler.RightArm.transform;
    //            transform2 = _bodyHandler.RightForearm.transform;
    //            rigidbody = _bodyHandler.RightArm.PartRigidbody;
    //            rigidbody2 = _bodyHandler.RightForearm.PartRigidbody;
    //            rigidbody3 = _bodyHandler.RightHand.PartRigidbody;
    //            vector = -_bodyHandler.Chest.transform.right;
    //            break;
    //    }
    //    if (!isDuck && !isKickDuck && !isRun)
    //    {
    //        switch (pose)
    //        {
    //            case Pose.Bent:
    //                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, -_moveDir / 4f, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //            case Pose.Forward:
    //                AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, _moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //            case Pose.Straight:
    //                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //            case Pose.Behind:
    //                AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //        }
    //        return;
    //    }
    //    switch (pose)
    //    {
    //        case Pose.Bent:
    //            AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //            AlignToVector(rigidbody2, transform2.forward, -_moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(-_moveDir * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody3.AddForce(_moveDir * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //        case Pose.Forward:
    //            AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce);
    //            AlignToVector(rigidbody2, transform2.forward, _moveDir + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //        case Pose.Straight:
    //            AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //            AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //        case Pose.Behind:
    //            AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
    //            AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //    }
    //}


    //private void RunCyclePoseBody()
    //{
    //    Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
    //    Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
    //    _moveDir = lookForward * MoveInput.z + lookRight * MoveInput.x;

    //    _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
    //    _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

    //    AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
    //    AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
    //    AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
    //    AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
    //    AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
    //    AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

    //    if (isRun)
    //    {
    //        _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime * 1.35f);
    //        if (_hips.velocity.magnitude > MaxSpeed)
    //            _hips.velocity = _hips.velocity.normalized * MaxSpeed * 1.15f;
    //    }
    //    else
    //    {
    //        _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime);
    //        if (_hips.velocity.magnitude > MaxSpeed)
    //            _hips.velocity = _hips.velocity.normalized * MaxSpeed;
    //    }

    //}
}
