using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BodyHandler : MonoBehaviourPun
{
    public Transform Root;
    public List<BodyPart> BodyParts = new List<BodyPart>(17);

    public BodyPart LeftFoot => GetBodyPart(Define.BodyPart.FootL);
    public BodyPart RightFoot => GetBodyPart(Define.BodyPart.FootR);
    public BodyPart LeftLeg => GetBodyPart(Define.BodyPart.LegLowerL);
    public BodyPart RightLeg => GetBodyPart(Define.BodyPart.LegLowerR);
    public BodyPart LeftThigh => GetBodyPart(Define.BodyPart.LegUpperL);
    public BodyPart RightThigh => GetBodyPart(Define.BodyPart.LegUpperR);

    public BodyPart Hip => GetBodyPart(Define.BodyPart.Hip);
    public BodyPart Waist => GetBodyPart(Define.BodyPart.Waist);
    public BodyPart Chest => GetBodyPart(Define.BodyPart.Chest);
    public BodyPart Head => GetBodyPart(Define.BodyPart.Head);


    public BodyPart LeftArm => GetBodyPart(Define.BodyPart.LeftArm);
    public BodyPart RightArm => GetBodyPart(Define.BodyPart.RightArm);
    public BodyPart LeftForeArm => GetBodyPart(Define.BodyPart.LeftForeArm);
    public BodyPart RightForeArm => GetBodyPart(Define.BodyPart.RightForeArm);
    public BodyPart LeftHand => GetBodyPart(Define.BodyPart.LeftHand);
    public BodyPart RightHand => GetBodyPart(Define.BodyPart.RightHand);

    public BodyPart Ball => GetBodyPart(Define.BodyPart.Ball);


    private void Awake()
    {
        if(BodyParts.Count ==0)
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


        foreach (BodyPart part in BodyParts)
        {
            part.PartSetup();
        }
    }

    private BodyPart GetBodyPart(Define.BodyPart part)
    {
        return BodyParts[(int)part];
    }
}

