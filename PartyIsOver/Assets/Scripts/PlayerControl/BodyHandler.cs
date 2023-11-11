using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BodyHandler : MonoBehaviour
{
    public Transform Root;
    public List<BodyPart> BodyParts = new List<BodyPart>();

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
    public BodyPart LeftForearm;
    public BodyPart RightForearm;
    public BodyPart LeftHand;
    public BodyPart RightHand;

    public BodyPart Ball;

    // PunRPC¿ë
    public BodyPart CurrentBodyPart;

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
        BodyParts.Add(LeftForearm);
        BodyParts.Add(LeftHand);

        BodyParts.Add(LeftThigh);
        BodyParts.Add(LeftLeg);
        BodyParts.Add(LeftFoot);

        BodyParts.Add(RightArm);
        BodyParts.Add(RightForearm);
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
