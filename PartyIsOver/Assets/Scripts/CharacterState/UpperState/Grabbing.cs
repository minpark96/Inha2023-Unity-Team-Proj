using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Grabbing : BodyState
{
    private UpperBodySM _sm;
    private PlayerContext _context;


    private float _grabDelayTimer =0;

    public bool _isGrabbingInProgress { get; private set; }


    public Grabbing(StateMachine stateMachine) : base("GrabbingState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }


    public override void Enter()
    {
        _context = _sm.Context;
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
        if (!Input.GetKey(KeyCode.Mouse0))
            _sm.ChangeState(_sm.IdleState);
    }

    public override void UpdatePhysics()
    {
    }

    public override void Exit()
    {
    }

    private void GrabbingFunc()
    {
        //서치액션
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.TargetSearch);

        //Debug.Log(_leftSearchTarget);
        //Debug.Log(_rightSearchTarget);

        //발견한 오브젝트가 없으면 리턴
        if (_context.LeftSearchTarget == null && _context.LeftSearchTarget == null)
            return;

        _isGrabbingInProgress = true;

        //타겟이 정면에 있고 아이템일때
        if (_context.LeftSearchTarget == _context.LeftSearchTarget && _context.LeftSearchTarget.IsItem)
        {
            _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);
            //일정 거리 안에서 양손이 비어있을때
            if (TargetingHandler.TargetDistance(TargetingHandler.FindClosestCollisionPoint
                (_context.Position,_context.LeftSearchTarget.ColliderObject,_context.Layer))<= 1f
                  && _context.LeftGrabObject ==null && _context.RightGrabObject != null)
            {
                //아이템 잡기 상태로 진입
                //HandleItemGrabbing(_context.LeftSearchTarget.ItemObject);
                return;
            }
        }
        else//아이템이 아닐때 혹은 손이 멀리 있을때
        {
            if (_context.LeftSearchTarget != null && _context.LeftGrabObject ==null)
            {
                Debug.Log("gabbing44");
                _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);

                //손이 닿았을때 포톤뷰가 있으면 id를 저장하고 각 캐릭터들에게 JointFix를 시킴
                if (HandCollisionCheck(Side.Left))
                {
                    Debug.Log("gabbing55");
                    _context.LeftGrabObject = _context.LeftSearchTarget;
                    _sm.InputHandler.EnqueueCommand(COMMAND_KEY.FixJoint);
                    _grabDelayTimer = 0.5f;
                }
            }

            if (_context.RightSearchTarget != null && _context.RightGrabObject == null)
            {
                //손 뻗기 Action 실행
                _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);

                if (HandCollisionCheck(Side.Right))
                {
                    _context.RightGrabObject = _context.RightSearchTarget;
                    _sm.InputHandler.EnqueueCommand(COMMAND_KEY.FixJoint);
                    _grabDelayTimer = 0.5f;
                }
            }
        }
    }



    //나중에 수정
    bool HandCollisionCheck(Define.Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (_sm.LeftHandCheckter.CollisionObject != null && //앞부분 null체크 지워도 될 것 같음
                   _sm.LeftHandCheckter.CollisionObject == _context.LeftSearchTarget)
                {
                    return true;
                }
                break;
            case Side.Right:
                if (_sm.RightHandCheckter.CollisionObject != null &&
                    _sm.RightHandCheckter.CollisionObject == _context.RightSearchTarget)
                {
                    return true;
                }
                break;
            case Side.Both:
                if (HandCollisionCheck(Side.Right) && HandCollisionCheck(Side.Left))
                    return true;
                break;
        }
        return false;
    }
   

    //void HandleItemGrabbing(Item item)
    //{
    //    switch (item.ItemData.ItemType)
    //    {
    //        case ItemType.OneHanded:
    //            {
    //                Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
    //                _rightHandRigid.AddForce(dir.normalized * 80f);

    //                if (IsHoldingItem(item, Side.Right))
    //                    ItemRotate(item.transform, false);
    //                else
    //                    return;
    //                //아이템에 맞게 관절조정 함수 추가해야함

    //                int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //                photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
    //            }
    //            break;
    //        case ItemType.TwoHanded:
    //            {
    //                TwoHandedGrab(item);
    //            }
    //            break;
    //        case ItemType.Ranged:
    //            {
    //                TwoHandedGrab(item);
    //            }
    //            break;
    //        case ItemType.Gravestone:
    //            {
    //                TwoHandedGrab(item);
    //            }
    //            break;
    //        case ItemType.Consumable:
    //            {
    //                Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
    //                _rightHandRigid.AddForce(dir.normalized * 80f);

    //                if (IsHoldingItem(item, Side.Right))
    //                    ItemRotate(item.transform, false);
    //                else
    //                    return;
    //                //아이템에 맞게 관절조정 함수 추가해야함

    //                int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //                photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
    //            }
    //            break;
    //    }
    //}
}
