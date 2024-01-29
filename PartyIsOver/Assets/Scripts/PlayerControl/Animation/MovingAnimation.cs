using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class MovingAnimation : MonoBehaviour
{
    private float MovingMotionSpeed;
    private float MovingMotionTimer = 0;
    private float maxSpeed = 2f;
    private float applyedForce = 800f;

    private bool isRun;

    BodyHandler bodyHandler;
    MovementSM movementSM;

    Vector3 runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    Vector3 runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    Vector3 runVectorForce10 = new Vector3(0f, 0f, 0.8f);
    Vector3 moveDir;

    Define.Pose leftLegPose;
    Define.Pose rightLegPose;
    Define.Pose leftArmPose;
    Define.Pose rightArmPose;

    private void Awake()
    {
        bodyHandler = GetComponent<BodyHandler>();
        movementSM = GetComponent<MovementSM>();
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

        Transform hip = bodyHandler.Hip.transform;
        Transform thighTrans = bodyHandler.LeftThigh.transform;
        Transform legTrans = bodyHandler.LeftLeg.transform;

        Rigidbody thighRigid = bodyHandler.LeftThigh.GetComponent<Rigidbody>();
        Rigidbody legRigid = bodyHandler.LeftLeg.PartRigidbody;

        if (side == Define.Side.Right)
        {
            thighTrans = bodyHandler.RightThigh.transform;
            legTrans = bodyHandler.RightLeg.transform;
            thighRigid = bodyHandler.RightThigh.PartRigidbody;
            legRigid = bodyHandler.RightLeg.PartRigidbody;
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
        Vector3 vector = bodyHandler.Chest.transform.right;
        Transform partTransform = bodyHandler.Chest.transform;
        Transform transform = bodyHandler.LeftArm.transform;
        Transform transform2 = bodyHandler.LeftForearm.transform;
        Rigidbody rigidbody = bodyHandler.LeftArm.PartRigidbody;
        Rigidbody rigidbody2 = bodyHandler.LeftForearm.PartRigidbody;
        Rigidbody rigidbody3 = bodyHandler.LeftHand.PartRigidbody;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;
        if (side == Define.Side.Right)
        {
            transform = bodyHandler.RightArm.transform;
            transform2 = bodyHandler.RightForearm.transform;
            rigidbody = bodyHandler.RightArm.PartRigidbody;
            rigidbody2 = bodyHandler.RightForearm.PartRigidbody;
            rigidbody3 = bodyHandler.RightHand.PartRigidbody;
            vector = -bodyHandler.Chest.transform.right;
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

    public void RunCyclePoseBody(Vector3 moveInput)
    {
        Vector3 lookForward = new Vector3(movementSM.PlayerCharacter.CameraTransform.forward.x, 0f, movementSM.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(movementSM.PlayerCharacter.CameraTransform.right.x, 0f, movementSM.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        bodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + moveDir), ForceMode.VelocityChange);
        bodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(bodyHandler.Chest.PartRigidbody, -bodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(bodyHandler.Chest.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, -bodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, -bodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

        if (isRun)
        {
            movementSM.Rigidbody.AddForce(moveDir.normalized * movementSM.Speed * Time.deltaTime * movementSM.RunSpeed);
            if (movementSM.Rigidbody.velocity.magnitude > maxSpeed)
                movementSM.Rigidbody.velocity = movementSM.Rigidbody.velocity.normalized * maxSpeed * 1.15f;
        }
        else
        {
            movementSM.Rigidbody.AddForce(moveDir.normalized * movementSM.Speed * Time.deltaTime);
            if (movementSM.Rigidbody.velocity.magnitude > maxSpeed)
                movementSM.Rigidbody.velocity = movementSM.Rigidbody.velocity.normalized * maxSpeed;
        }
    }

    void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
    {
        if (part == null)
        {
            return;
        }
        Vector3 vector = Vector3.Cross(Quaternion.AngleAxis(part.angularVelocity.magnitude * 57.29578f * stability / speed, part.angularVelocity) * alignmentVector, targetVector * 10f);
        if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z))
        {
            part.AddTorque(vector * speed * speed);
            {
                Debug.DrawRay(part.position, alignmentVector * 0.2f, Color.red, 0f, depthTest: false);
                Debug.DrawRay(part.position, targetVector * 0.2f, Color.green, 0f, depthTest: false);
            }
        }
    }
}
