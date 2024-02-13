using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingAction
{
    public GrabbingAction(ActionController actions)
    {
        actions.OnGrabbing -= HandleGrabbingEvent;
        actions.OnGrabbing += HandleGrabbingEvent;
        this._actions = actions;
    }

    ActionController _actions;
    AnimationPlayer _animPlayer;
    //BodyHandler _bodyHandler;
    AnimationData _animData;
    Vector3 _moveDir = new Vector3();


    private TargetingHandler _targetingHandler;
    private InteractableObject _leftSearchTarget;
    private InteractableObject _rightSearchTarget;



    public bool HandleGrabbingEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in Define.PlayerDynamicData data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        //_bodyHandler = bodyhandler;

        _moveDir.x = data.dirX;
        _moveDir.y = data.dirY;
        _moveDir.z = data.dirZ;

        _targetingHandler = new TargetingHandler();




        return true;
    }

    //private void Grabbing()
    //{
    //    if (_grabDelayTimer > 0f)
    //        return;

    //    SearchTarget();
    //    //Debug.Log(_leftSearchTarget);
    //    //Debug.Log(_rightSearchTarget);

    //    //�߰��� ������Ʈ�� ������ ����
    //    if (_leftSearchTarget == null && _rightSearchTarget == null)
    //        return;

    //    _isGrabbingInProgress = true;

    //    //Ÿ���� ���鿡 �ְ� �������϶�
    //    if (_leftSearchTarget == _rightSearchTarget && _leftSearchTarget.GetComponent<Item>() != null)
    //    {
    //        //���� �Ÿ� �̳��� ������ ����� ���������
    //        if (Vector3.Distance(_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>()),
    //            _actor.BodyHandler.Chest.transform.position) <= 1f
    //              && !_isRightGrab && !_isLeftGrab)
    //        {
    //            Item item = _leftSearchTarget.GetComponent<Item>();
    //            HandleItemGrabbing(item);
    //            return;
    //        }
    //    }
    //    else//�������� �ƴҶ�
    //    {
    //        Vector3 dir;
    //        //Ÿ���� ������ �ƴҶ�
    //        if (_leftSearchTarget != null && !_isLeftGrab)
    //        {
    //            if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
    //            {
    //                dir = ((_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>()) + Vector3.up * 2)
    //                    - _leftHandRigid.transform.position).normalized;
    //            }
    //            else
    //            {
    //                dir = (_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>())
    //                    - _leftHandRigid.transform.position).normalized;
    //            }

    //            _leftHandRigid.AddForce(dir * 80f);
    //            if (HandCollisionCheck(Side.Left))
    //            {
    //                int leftObjViewID = -1;
    //                if (_leftSearchTarget.GetComponent<PhotonView>() != null)
    //                {
    //                    leftObjViewID = _leftSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //                }
    //                photonView.RPC("JointFix", RpcTarget.All, (int)Side.Left, leftObjViewID);
    //                _grabDelayTimer = 0.5f;
    //            }
    //        }

    //        if (_rightSearchTarget != null && !_isRightGrab)
    //        {
    //            if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
    //            {
    //                dir = ((_targetingHandler.FindClosestCollisionPoint(_rightSearchTarget.GetComponent<Collider>()) + Vector3.up * 2)
    //                    - _rightHandRigid.transform.position).normalized;
    //            }
    //            else
    //            {
    //                dir = (_targetingHandler.FindClosestCollisionPoint(_rightSearchTarget.GetComponent<Collider>())
    //                    - _rightHandRigid.transform.position).normalized;
    //            }

    //            _rightHandRigid.AddForce(dir * 80f);
    //            if (HandCollisionCheck(Side.Right))
    //            {
    //                int rightObjViewID = -1;
    //                if (_rightSearchTarget.GetComponent<PhotonView>() != null)
    //                {
    //                    rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //                }
    //                photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
    //                _grabDelayTimer = 0.5f;
    //            }
    //        }
    //    }
    //}


    //private void SearchTarget()
    //{
    //    //Ÿ�ټ�ġ �±׼��� �����Ұ�
    //    _leftSearchTarget = _targetingHandler.SearchTarget(Side.Left);
    //    _rightSearchTarget = _targetingHandler.SearchTarget(Side.Right);

    //    if (_leftSearchTarget != null && _leftSearchTarget.GetComponent<PhotonView>() != null)
    //    {
    //        int id = _leftSearchTarget.GetComponent<PhotonView>().ViewID;
    //        photonView.RPC("BroadcastFoundTarget", RpcTarget.All, 0, id);

    //    }
    //    if (_rightSearchTarget != null && _rightSearchTarget.GetComponent<PhotonView>() != null)
    //    {
    //        int id = _rightSearchTarget.GetComponent<PhotonView>().ViewID;
    //        photonView.RPC("BroadcastFoundTarget", RpcTarget.All, 1, id);

    //    }
    //}
}
