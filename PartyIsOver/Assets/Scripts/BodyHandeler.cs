using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BodyHandeler : MonoBehaviour
{
    public Transform Root;
    private List<BodyPart> _bodyParts = new List<BodyPart>();



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



    // Start is called before the first frame update
    void Start()
    {

        BodySetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BodySetup()
    {

        _bodyParts.Add(Head);
        _bodyParts.Add(Chest);
        _bodyParts.Add(Waist);
        _bodyParts.Add(Hip);
        _bodyParts.Add(LeftArm);
        _bodyParts.Add(LeftForarm);
        _bodyParts.Add(LeftHand);

        _bodyParts.Add(LeftThigh);
        _bodyParts.Add(LeftLeg);
        _bodyParts.Add(LeftFoot);
        _bodyParts.Add(RightArm);
        _bodyParts.Add(RightForarm);
        _bodyParts.Add(RightHand);

        _bodyParts.Add(RightThigh);
        _bodyParts.Add(RightLeg);
        _bodyParts.Add(RightFoot);
        _bodyParts.Add(Ball);


        foreach (BodyPart part in _bodyParts)
        {
            part.PartRigidbody.maxAngularVelocity = 15f;
            part.PartRigidbody.solverIterations = 12;
            part.PartRigidbody.solverVelocityIterations = 12;

        }
    }
}
