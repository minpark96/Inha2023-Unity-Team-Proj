using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BodyHandler : MonoBehaviourPun
{
    public Transform Root;
    public List<BodyPart> BodyParts = new List<BodyPart>(17);

    public BodyPart LeftFoot;
    public BodyPart RightFoot;
    public BodyPart LeftLeg;
    public BodyPart RightLeg;
    public BodyPart LeftThigh;
    public BodyPart RightThigh;

    public BodyPart Hip;
    public BodyPart Waist;
    public BodyPart Chest;
    public BodyPart Head;

    public BodyPart LeftArm;
    public BodyPart RightArm;
    public BodyPart LeftForeArm;
    public BodyPart RightForeArm;
    public BodyPart LeftHand;
    public BodyPart RightHand;

    public BodyPart Ball => GetBodyPart(Define.BodyPart.Ball);


    public ConfigurableJoint[] ChildJoints;
    public ConfigurableJointMotion[] OriginalYMotions;
    public ConfigurableJointMotion[] OriginalZMotions;
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    private void Awake()
    {
        InitBodyParts();
        SaveConfigurableJoint();
    }

    private void InitBodyParts()
    {
        if (BodyParts.Count == 0)
        {
            foreach (Transform child in transform)
            {
                var component = child.GetComponent<BodyPart>();
                if (component != null)
                {
                    BodyParts.Add(component);
                }
            }
        }

        for (int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].PartSetup((Define.BodyPart)i);
        }

        LeftFoot = GetBodyPart(Define.BodyPart.FootL);
        RightFoot = GetBodyPart(Define.BodyPart.FootR);
        LeftLeg = GetBodyPart(Define.BodyPart.LegLowerL);
        RightLeg = GetBodyPart(Define.BodyPart.LegLowerR);
        LeftThigh = GetBodyPart(Define.BodyPart.LegUpperL);
        RightThigh = GetBodyPart(Define.BodyPart.LegUpperR);

        Hip = GetBodyPart(Define.BodyPart.Hip);
        Waist = GetBodyPart(Define.BodyPart.Waist);
        Chest = GetBodyPart(Define.BodyPart.Chest);
        Head = GetBodyPart(Define.BodyPart.Head);


        LeftArm = GetBodyPart(Define.BodyPart.LeftArm);
        RightArm = GetBodyPart(Define.BodyPart.RightArm);
        LeftForeArm = GetBodyPart(Define.BodyPart.LeftForeArm);
        RightForeArm = GetBodyPart(Define.BodyPart.RightForeArm);
        LeftHand = GetBodyPart(Define.BodyPart.LeftHand);
        RightHand = GetBodyPart(Define.BodyPart.RightHand);
    }

    private void SaveConfigurableJoint()
    {
        ChildJoints = GetComponentsInChildren<ConfigurableJoint>();
        OriginalYMotions = new ConfigurableJointMotion[ChildJoints.Length];
        OriginalZMotions = new ConfigurableJointMotion[ChildJoints.Length];

        // 원래의 angularMotion 값을 저장
        for (int i = 0; i < ChildJoints.Length; i++)
        {
            OriginalYMotions[i] = ChildJoints[i].angularYMotion;
            OriginalZMotions[i] = ChildJoints[i].angularZMotion;
        }

        //초기 spring값 저장
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    private BodyPart GetBodyPart(Define.BodyPart part)
    {
        return BodyParts[(int)part];
    }

    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //기절과 회복에 모두 관여 기절시엔 퍼센티지를 0으로해서 사용
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }
    }

    public IEnumerator ResetBodySpring()
    {
        SetJointSpring(0f);
        yield return null;
    }

    public IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time - startTime < springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            SetJointSpring(percentage);
            yield return null;
        }
    }

    public void JointLock(Define.Side side)
    {
        ConfigurableJoint hand = (side == Define.Side.Left)? LeftHand.PartJoint : RightHand.PartJoint;
        ConfigurableJoint ForeArm = (side == Define.Side.Left) ? LeftForeArm.PartJoint : RightForeArm.PartJoint;
        ConfigurableJoint UpperArm = (side == Define.Side.Left) ? LeftArm.PartJoint : RightArm.PartJoint;

        hand.angularYMotion = ConfigurableJointMotion.Locked;
        ForeArm.angularYMotion = ConfigurableJointMotion.Locked;
        UpperArm.angularYMotion = ConfigurableJointMotion.Locked;
        hand.angularZMotion = ConfigurableJointMotion.Locked;
        ForeArm.angularZMotion = ConfigurableJointMotion.Locked;
        UpperArm.angularZMotion = ConfigurableJointMotion.Locked;
    }
}

