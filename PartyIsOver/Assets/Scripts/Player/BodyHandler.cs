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

    //public Transform Spring;

    private bool _isSetting = false;

    public void BodySetup()
    {
        if (_isSetting)
            return;

        _isSetting = true;

        if (BodyParts.Count == 0 )
        {
            BodyParts.Add(LeftFoot);
            BodyParts.Add(RightFoot);

            BodyParts.Add(LeftLeg);
            BodyParts.Add(RightLeg);

            BodyParts.Add(LeftThigh);
            BodyParts.Add(RightThigh);

            BodyParts.Add(Hip);
            BodyParts.Add(Waist);
            BodyParts.Add(Chest);
            BodyParts.Add(Head);

            BodyParts.Add(LeftArm);
            BodyParts.Add(RightArm);

            BodyParts.Add(LeftForearm);
            BodyParts.Add(RightForearm);

            BodyParts.Add(LeftHand);
            BodyParts.Add(RightHand);

            BodyParts.Add(Ball);
        }

        foreach (BodyPart part in BodyParts)
        {
            part.PartRigidbody.maxAngularVelocity = 15f;
            part.PartRigidbody.solverIterations = 12;
            part.PartRigidbody.solverVelocityIterations = 12;
            part.GetComponent<PhotonRigidbodyView>().m_SynchronizeVelocity = true;
            part.GetComponent<PhotonRigidbodyView>().m_SynchronizeAngularVelocity = true;
        }
    }
}
