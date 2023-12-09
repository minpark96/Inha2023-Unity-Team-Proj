using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static Define;

public class Stun : MonoBehaviourPun ,IState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }

    private bool _isStun = false;
    private GameObject effectObject = null;
    private Transform playerTransform;

    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    public void EnterState()
    {
        _isStun = true;

        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();

        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }

        InstantiateEffect("Effects/Stun_loop");

        photonView.RPC("ResetBodySpring", RpcTarget.All);
    }

    public void UpdateState()
    {
        if(effectObject != null)
        {
            effectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
        }
    }

    public void ExitState()
    {
        _isStun = false;

        StartCoroutine(RestoreBodySpring(0.07f));

        MyActor.actorState = Actor.ActorState.Stand;
        MyActor.InvokeStatusChangeEvent();

        photonView.RPC("RemoveObject", RpcTarget.All, "Stun_loop");

    }

    [PunRPC]
    IEnumerator ResetBodySpring()
    {
        photonView.RPC("SetJointSpring", RpcTarget.All, 0f);
        yield return null;
    }

    [PunRPC]
    IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

    [PunRPC]
    public void InstantiateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
    }

    [PunRPC]
    public void RemoveObject(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

    [PunRPC]
    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //기절과 회복에 모두 관여 기절시엔 퍼센티지를 0으로해서 사용
        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }
    }
}
