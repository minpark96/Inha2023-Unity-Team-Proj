using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 플레이어, 오브젝트, 벽 등을 잡기 진행중인 상태 
public class Grabbing : BaseState
{
    private UpperBodySM _sm;
    private PlayerActionContext _context;


    private float _grabDelayTimer =0;
    private float _itemSearchRange = 1f;

    public Grabbing(StateMachine stateMachine) : base(PlayerState.Grabbing, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }


    public override void Enter()
    {
        _context = _sm.PlayerContext;
        _sm.IsGrabbingInProgress = true;
    }

    public override void UpdateLogic()
    {
        _grabDelayTimer -= Time.deltaTime;
        
        // 컨텍스트에서 양 손에 잡고 있는 오브젝트를 가져온다.
        if (_context.RightGrabObject != null && _context.LeftGrabObject != null
            && _context.RightGrabObject.ItemObject == null)
        {
            //잡은 오브젝트 종류에 따라 다른 상태로 변환
            if(_context.RightGrabObject.Type == ObjectType.Wall)
                _sm.ChangeState(_sm.StateMap[PlayerState.Climb]);
            else
                _sm.ChangeState(_sm.StateMap[PlayerState.LiftObject]);
        }
    }
    public override void GetInput()
    {
        //마우스 떼면 관절을 제거함과 동시에 잡기를 해제하고 Idle 상태로
        if(!InputCommand(COMMAND_KEY.LeftBtn, KeyType.Press))
        {
            InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
    }

    public override void UpdatePhysics()
    {
        // 초당 60번씩 오브젝트 탐색
       InvokeReserveCommand(COMMAND_KEY.TargetSearch);
        // 그래빙Action 계속 실행
        if (_grabDelayTimer < 0f)
            GrabbingProgress();
    }

    public override void Exit()
    {
        _sm.IsGrabbingInProgress = false;
    }
    
    // 잡기 진행
    private void GrabbingProgress()
    {

        //Debug.Log(_context.LeftSearchTarget);
        //Debug.Log(_context.RightSearchTarget);

        //발견한 오브젝트가 없으면 리턴
        if (_context.LeftSearchTarget == null && _context.RightSearchTarget == null)
            return;
        Debug.Log("grabbing");


        //타겟이 정면에 있고 아이템일때
        if (_context.LeftSearchTarget == _context.RightSearchTarget && _context.RightSearchTarget.ItemObject !=null)
        {
            //일정 거리 안에서 양손이 비어있을때
            if (IsTargetInRange() && IsHandsEmpty())
            {
                //아이템 잡기 상태로 진입
                _context.IsItemGrabbing = true;
                if (IsItemGrabbing(_context.RightSearchTarget))
                    _sm.ChangeState(_sm.StateMap[PlayerState.EquipItem]);
                else
                    InvokeReserveCommand(COMMAND_KEY.Grabbing);

                return;
            }
            else
                _context.IsItemGrabbing = false;
        }
        else//타겟이 정면에 없거나 아이템이 아닐 경우
        {
            if (_context.LeftSearchTarget != null && _context.LeftGrabObject ==null)
            {
                if (HandCollisionCheck(Side.Left))
                {
                    InvokeReserveCommand(COMMAND_KEY.FixJoint);
                    _grabDelayTimer = 0.5f;
                }
                else
                    InvokeReserveCommand(COMMAND_KEY.Grabbing);
            }

            if (_context.RightSearchTarget != null && _context.RightGrabObject == null)
            {
                //손 뻗기 Action 실행 및 닿았는지 체크
                if (HandCollisionCheck(Side.Right))
                {
                    InvokeReserveCommand(COMMAND_KEY.FixJoint);
                    _grabDelayTimer = 0.5f;
                }
                else
                    InvokeReserveCommand(COMMAND_KEY.Grabbing);
            }
        }
    }


    bool IsTargetInRange()
    {
        if (TargetingHandler.TargetDistance(TargetingHandler.FindClosestCollisionPoint
                (_context.Position, _context.RightSearchTarget.ColliderObject, _context.Layer)) <= _itemSearchRange)
            return true;
        else return false;
    }

    bool IsHandsEmpty()
    {
        if (_context.LeftGrabObject == null && _context.RightGrabObject == null)
            return true;
        else return false;
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
    
    // 아이템 잡기일 경우 아이템 종류에 따라 분기처리
    bool IsItemGrabbing(InteractableObject item)
    {
        switch (item.ItemObject.ItemData.ItemType)
        {
            case ItemType.TwoHanded:
                    if (!IsHoldingItem(item, Define.Side.Both))
                        return false;
                break;
            case ItemType.Ranged:
                    if (!IsHoldingItem(item, Define.Side.Both))
                        return false;
                break;
            case ItemType.Consumable:
                    if (!IsHoldingItem(item, Side.Right))
                        return false;
                break;
            default:
                return false;
        }

        if(item.ItemObject.ItemData.ItemType != ItemType.Consumable)
            _context.ItemHandleSide = ItemDirCheck(item.ItemObject);
        return true;
    }

    // 아이템과 양손이 접촉중인지 체크하는 함수
    bool IsHoldingItem(InteractableObject item, Define.Side side)
    {
        //HandChecker 스크립트에서 양손 다 아이템의 손잡이와 접촉중인지 판정
        if (HandCollisionCheck(side))
        {
            _grabDelayTimer = 0.5f;
            _sm.PlayerContext.EquipItem = item;
            return true;
        }
        return false;
    }

    // 아이템을 잡은 방향을 리턴하는 함수
    Side ItemDirCheck(Item item)
    {
        //오른손과 손잡이 위치 체크해서 아이템 방향 리턴
        Vector3 toItem = (item.TwoHandedPos.position - _context.Position).normalized; // 플레이어가 아이템을 바라보는 벡터
        Vector3 toOneHandedHandle = (item.OneHandedPos.position - _context.Position).normalized; // 오른손이 잡아야할 oneHanded 손잡이 벡터
        Vector3 crossProduct = Vector3.Cross(toItem, toOneHandedHandle);

        if (crossProduct.y > 0)
            return Side.Right;// 원핸드 손잡이가 오른쪽
        else
            return Side.Left;// 원핸드 손잡이가 왼쪽
    }

}
