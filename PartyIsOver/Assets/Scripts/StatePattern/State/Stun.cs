using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static Define;

public class Stun : MonoBehaviourPun ,IState
{
    private bool _isStun = false;
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject = null;
    public Transform playerTransform;

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

        //MyActor.debuffState = Actor.DebuffState.Stun;
        //TODO :: 이펙트 생성 추가
        photonView.RPC("InstantiateEffect", RpcTarget.All, "Effects/Stun_loop");
        //InstantiateEffect("Effects/Stun_loop");

        Debug.Log("Start ResetBodySpring");
        StartCoroutine("ResetBodySpring");
        Debug.Log($"상태를 변환 했습니다. {_isStun}");
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

        Debug.Log("상태를 정상으로 돌립니다 : " + _isStun);
        //StartCoroutine이여서 병렬로 돌아서 찍고 다시 돌아옴
        StartCoroutine(RestoreBodySpring(0.07f));
        MyActor.actorState = Actor.ActorState.Stand;

        /*MyActor.actorState = Actor.ActorState.Stand;
        MyActor.debuffState &= ~Actor.DebuffState.Stun;*/

        MyActor.InvokeStatusChangeEvent();
        //TODO :: 이펙트 삭제
        photonView.RPC("RemoveObject", RpcTarget.All, "Stun_loop");
        //RemoveObject("Stun_loop");

        //여기서 지금 자기 자신의 상태를 널로 해야한야하는데
        //지금 상태를 여기서 바꾸던지 해야하는데

    }

    IEnumerator ResetBodySpring()
    {
        photonView.RPC("SetJointSpring", RpcTarget.All, 0f);
        yield return null;
    }

    IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        Debug.Log("Start RestoreBodySpring");
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            //SetJointSpring(percentage);
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            Debug.Log("RestoreBodySpring+++++++++++++++");
            yield return null;
        }
        Debug.Log("End RestoreBodySpring");
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

        Debug.Log("Start SetJointSpring");

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
        Debug.Log("End SetJointSpring");
    }
}
