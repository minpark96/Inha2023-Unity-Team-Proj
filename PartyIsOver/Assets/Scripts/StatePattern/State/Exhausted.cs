using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhausted : MonoBehaviourPun, IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }
    JointDrive angularXDrive;
    private List<float> _xPosSpringAry = new List<float>();

    public void EnterState()
    {
        effectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
        }

        InstantiateEffect("Effects/Wet");

        angularXDrive = MyActor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive;
        angularXDrive.positionSpring = 0f;
        MyActor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive = angularXDrive;
    }

    public void UpdateState()
    {
        if (effectObject != null)
        {
            effectObject.transform.position = playerTransform.position;
        }
    }

    public void ExitState()
    {
        MyActor.debuffState &= Actor.DebuffState.Exhausted;
        RemoveObject("Wet");

        angularXDrive.positionSpring = _xPosSpringAry[0];
        MyActor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive = angularXDrive;
        MyActor.InvokeStatusChangeEvent();

    }
    public void InstantiateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
    }
    public void RemoveObject(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

}
