using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BodyHandler : MonoBehaviour
{
    public Transform Root;
    public List<BodyPart> BodyParts = new List<BodyPart>();



    public BodyPart Head;

    public BodyPart Chest;

    public BodyPart Waist;

    public BodyPart Hip;



    public BodyPart LeftArm;

    public BodyPart LeftForarm;

    public BodyPart LeftHand;

    public BodyPart LeftThigh;

    public BodyPart LeftLeg;

    public BodyPart LeftFoot;



    public BodyPart RightArm;

    public BodyPart RightForarm;

    public BodyPart RightHand;

    public BodyPart RightThigh;

    public BodyPart RightLeg;

    public BodyPart RightFoot;

    public BodyPart Ball;

    //public Transform Spring;

    private bool _isSetting = false;

    public void BodySetup()
    {
        if (_isSetting)
            return;

        _isSetting = true;

        BodyParts.Add(Head);
        BodyParts.Add(Chest);
        BodyParts.Add(Waist);
        BodyParts.Add(Hip);

        BodyParts.Add(LeftArm);
        BodyParts.Add(LeftForarm);
        BodyParts.Add(LeftHand);

        BodyParts.Add(LeftThigh);
        BodyParts.Add(LeftLeg);
        BodyParts.Add(LeftFoot);

        BodyParts.Add(RightArm);
        BodyParts.Add(RightForarm);
        BodyParts.Add(RightHand);

        BodyParts.Add(RightThigh);
        BodyParts.Add(RightLeg);
        BodyParts.Add(RightFoot);
        BodyParts.Add(Ball);

        foreach (BodyPart part in BodyParts)
        {
            part.PartRigidbody.maxAngularVelocity = 15f;
            part.PartRigidbody.solverIterations = 12;
            part.PartRigidbody.solverVelocityIterations = 12;
        }
    }
}
