using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviour, IState
{
    private bool _isStun = false;
    public Actor MyActor { get; set; }
    private float _stateDownTime = 2;
    private float _stateUpTime = 2;

    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();



    public void EnterState()
    {
        _isStun = true;

            MyActor = GetComponent<Actor>();



        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }

        MyActor.debuffState = Actor.DebuffState.Stun;
        //TODO :: 이펙트 추가 

        ResetBodySpring();
        Debug.Log($"상태를 변환 했습니다. {_isStun}");
    }

    public void UpdateState()
    {
        if (_stateDownTime > 0f)
        {
            _stateDownTime -= Time.deltaTime;
            Debug.Log("지속시간 : " + _stateDownTime);
        }

        if (_stateDownTime <= 0f)
        {
            Debug.Log("시간이 끝났어요");

            ExitState();
            _stateDownTime = _stateUpTime;
            Debug.Log("지속시간 대기 채우기");
        }
    }

    public void ExitState()
    {
        _isStun = false;
        Debug.Log("상태를 정상으로 돌립니다 : " + _isStun);
        StartCoroutine(RestoreBodySpring());
        MyActor.actorState = Actor.ActorState.Stand;
        MyActor.debuffState &= ~Actor.DebuffState.Stun;
        MyActor.InvokeStatusChangeEvent();
        //TODO :: 이펙트 삭제
    }

    void ResetBodySpring()
    {
        SetJointSpring(0f);
        //photonView.RPC("SetJointSpring", RpcTarget.All, 0f);
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

    public IEnumerator RestoreBodySpring()
    {
        float startTime = Time.time;
        float springLerpDuration = 0.07f;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            SetJointSpring(percentage);
            //photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

}
