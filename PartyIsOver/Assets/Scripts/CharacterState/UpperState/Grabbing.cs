using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Grabbing : BodyState
{
    private UpperBodySM _sm;
    private TargetingHandler _targetingHandler;


    private float _grabDelayTimer =0;

    public bool _isGrabbingInProgress { get; private set; }

    bool _isRightGrab = false;
    bool _isLeftGrab = false;

    public Grabbing(StateMachine stateMachine) : base("GrabbingState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
        _targetingHandler = new TargetingHandler();
        
    }


    public override void Enter()
    {
    }

    public override void UpdateLogic()
    {
        _grabDelayTimer -= Time.deltaTime;


        //그래빙Action 계속 실행
        if (_grabDelayTimer < 0f)
            GrabbingFunc();

        //각각 손에 뭔가 잡힌것을 _sm이 가지고 있어야 하고 그걸 판단하다 해당상태로 이동
        //1.아이템인지, 벽인지, 플레이어인지 등


    }
    public override void GetInput()
    {
        //마우스 떼면 Idle로
    }

    public override void UpdatePhysics()
    {
    }

    public override void Exit()
    {
    }

    private void GrabbingFunc()
    {
        SearchTarget();
        //Debug.Log(_leftSearchTarget);
        //Debug.Log(_rightSearchTarget);

        //발견한 오브젝트가 없으면 리턴
        if (_sm.LeftSearchTarget == null && _sm.RightSearchTarget == null)
            return;

        _isGrabbingInProgress = true;

        //타겟이 정면에 있고 아이템일때
        if (_sm.LeftSearchTarget == _sm.RightSearchTarget && _sm.LeftSearchTarget.IsItem)
        {
            //일정 거리 이내에 있을때 양손이 비어있을때
            if (_sm.TargetingHandler.TargetDistance(_targetingHandler.FindClosestCollisionPoint(_sm.LeftSearchTarget.ColliderObject))<= 1f
                  && !_isRightGrab && !_isLeftGrab)
            {
                //아이템 잡기 상태로 진입
                HandleItemGrabbing(_sm.LeftSearchTarget.ItemObject);
                return;
            }
        }
        else//아이템이 아닐때 혹은 손이 멀리 있을때
        {
            //왼손 뻗기
            if (_sm.LeftSearchTarget != null && !_isLeftGrab)
            {
                //손 뻗기 Action 실행

                //손이 닿았을때 포톤뷰가 있으면 id를 저장하고 각 캐릭터들에게 JointFix를 시킴
                if (HandCollisionCheck(Side.Left))
                {
                    int leftObjViewID = -1;
                    if (_sm.LeftSearchTarget.GetComponent<PhotonView>() != null)
                    {
                        leftObjViewID = _sm.LeftSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    }
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Left, leftObjViewID);
                    _grabDelayTimer = 0.5f;
                }
            }

            if (_sm.RightSearchTarget != null && !_isRightGrab)
            {
                //손 뻗기 Action 실행

                if (HandCollisionCheck(Side.Right))
                {
                    int rightObjViewID = -1;
                    if (_sm.RightSearchTarget.GetComponent<PhotonView>() != null)
                    {
                        rightObjViewID = _sm.RightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    }
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                    _grabDelayTimer = 0.5f;
                }
            }
        }
    }


    bool HandCollisionCheck(Define.Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (_sm.LeftHandCheckter.CollisionObject != null &&
                   _sm.LeftHandCheckter.CollisionObject == _sm.LeftSearchTarget.gameObject)
                {
                    _isLeftGrab = true;
                    return true;
                }
                break;
            case Side.Right:
                if (_sm.RightHandCheckter.CollisionObject != null &&
                    _sm.RightHandCheckter.CollisionObject == _sm.RightSearchTarget.gameObject)
                {
                    _isRightGrab = true;
                    return true;
                }
                break;
            case Side.Both:
                if (HandCollisionCheck(Side.Right) && HandCollisionCheck(Side.Left))
                {
                    return true;
                }
                else
                {
                    _isRightGrab = false;
                    _isLeftGrab = false;
                }
                break;
        }
        return false;
    }

    private void SearchTarget()
    {
        //타겟서치 태그설정 주의할것
        _sm.LeftSearchTarget = _targetingHandler.SearchTarget(Side.Left);
        _sm.RightSearchTarget = _targetingHandler.SearchTarget(Side.Right);

        if (_sm.LeftSearchTarget != null && _sm.LeftSearchTarget.IsPhotonView)
        {
            int id = _sm.LeftSearchTarget.PhotonView.ViewID;
            photonView.RPC("BroadcastFoundTarget", RpcTarget.All, 0, id);
        }
        if (_sm.RightSearchTarget != null && _sm.RightSearchTarget.IsPhotonView)
        {
            int id = _sm.RightSearchTarget.PhotonView.ViewID;
            photonView.RPC("BroadcastFoundTarget", RpcTarget.All, 1, id);
        }

        if (_sm.LeftSearchTarget != null)
            _sm.LeftTargetDir = _targetingHandler.FindClosestCollisionPoint(_sm.LeftSearchTarget.ColliderObject);
        else
            _sm.LeftTargetDir = Vector3.zero;

        if (_sm.RightSearchTarget != null)
            _sm.RightTargetDir = _targetingHandler.FindClosestCollisionPoint(_sm.RightSearchTarget.ColliderObject);
        else
            _sm.RightTargetDir = Vector3.zero;
    }


    [PunRPC]
    private void BroadcastFoundTarget(int side, int id)
    {
        if (side == 0)
        {
            _sm.LeftSearchTarget = PhotonNetwork.GetPhotonView(id).transform.GetComponent<InteractableObject>();
            Debug.Log("target");
        }
        else if (side == 1)
        {
            _sm.RightSearchTarget = PhotonNetwork.GetPhotonView(id).transform.GetComponent<InteractableObject>();
        }
    }

    [PunRPC]
    void JointFix(int side, int objViewID = -1)
    {
        ItemType type = ItemType.None;
        if (_sm.EquipItem != null)
        {
            type = _sm.EquipItem.ItemObject.ItemData.ItemType;
            _sm.EquipItem.gameObject.layer = gameObject.layer;
        }

        //objViewID 는 그랩오브젝트의 ID
        PhotonView pv = PhotonNetwork.GetPhotonView(objViewID);
        if (photonView.IsMine && pv != null && EquipItem != null)
        {
            int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            pv.TransferOwnership(playerID);
        }

        //관절 생성 및 일부 고정
        if ((Define.Side)side == Define.Side.Left)
        {
            if (_sm.LeftSearchTarget == null)
            {
                Debug.Log("Fail LeftJointFix");
            }

            //여기가 문제
            _sm.LeftGrabJoint = _leftHandRigid.AddComponent<FixedJoint>();


            _sm.LeftGrabJoint.connectedBody = _sm.LeftSearchTarget.RigidbodyObject;
            //_grabJointLeft.breakForce = 9999999;

            //if (pv != null)
            //    _leftSearchTarget = pv.transform.GetComponent<InteractableObject>();


            if (_sm.LeftSearchTarget != null)
                _sm.LeftGrabObject = _sm.LeftSearchTarget;

            //액션으로 추출
            if (EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            {
                _jointLeft.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeft.angularZMotion = ConfigurableJointMotion.Locked;
                _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
                //_grabJointLeft.breakForce = 9999999;
            }

        }
        else if ((Define.Side)side == Define.Side.Right)
        {
            if (_sm.RightSearchTarget == null)
            {
                Debug.Log("Fail RightJointFix");
            }

            //여기가 문제
            _sm.RightGrabJoint = _rightHandRigid.AddComponent<FixedJoint>();

            //if (pv != null)
            //    _rightSearchTarget = pv.transform.GetComponent<InteractableObject>();

            _sm.RightGrabJoint.connectedBody = _sm.RightSearchTarget.RigidbodyObject;
            //_grabJointRight.breakForce = 9001;

            if (_sm.RightSearchTarget != null)
                _sm.RightGrabObject = _sm.RightSearchTarget;

            //액션으로 추출
            if (EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            {
                _jointRight.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRight.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
                //_grabJointRight.breakForce = 9999999;
            }
        }
    }

    void HandleItemGrabbing(Item item)
    {
        switch (item.ItemData.ItemType)
        {
            case ItemType.OneHanded:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir.normalized * 80f);

                    if (IsHoldingItem(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //아이템에 맞게 관절조정 함수 추가해야함

                    int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                }
                break;
            case ItemType.TwoHanded:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Ranged:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Gravestone:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Consumable:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir.normalized * 80f);

                    if (IsHoldingItem(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //아이템에 맞게 관절조정 함수 추가해야함

                    int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                }
                break;
        }
    }
}
