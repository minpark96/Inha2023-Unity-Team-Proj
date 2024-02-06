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
    }

    private BodyPart GetBodyPart(Define.BodyPart part)
    {
        return BodyParts[(int)part];
    }
}

