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
    private float _itemSearchRange = 1f;

    public Grabbing(StateMachine stateMachine) : base(PlayerState.Grabbing, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }


    public override void Enter()
    {
        _context = _sm.Context;
        _sm.IsGrabbingInProgress = true;
    }

    public override void UpdateLogic()
    {
        _grabDelayTimer -= Time.deltaTime;
    }
    public override void GetInput()
    {
        //���콺 ���� Idle��
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            _sm.InputHandler.EnqueueCommand(COMMAND_KEY.DestroyJoint);
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
           
    }

    public override void UpdatePhysics()
    {
        //�׷���Action ��� ����
        if (_grabDelayTimer < 0f)
            GrabbingProgress();
    }

    public override void Exit()
    {
        _sm.IsGrabbingInProgress = false;
    }

    private void GrabbingProgress()
    {
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.TargetSearch);

        //Debug.Log(_context.LeftSearchTarget);
        //Debug.Log(_context.RightSearchTarget);

        //�߰��� ������Ʈ�� ������ ����
        if (_context.LeftSearchTarget == null && _context.RightSearchTarget == null)
            return;


        //Ÿ���� ���鿡 �ְ� �������϶�
        if (_context.LeftSearchTarget == _context.RightSearchTarget && _context.RightSearchTarget.ItemObject !=null)
        {
            //���� �Ÿ� �ȿ��� ����� ���������
            if (TargetingHandler.TargetDistance(TargetingHandler.FindClosestCollisionPoint
                (_context.Position,_context.RightSearchTarget.ColliderObject,_context.Layer))<= _itemSearchRange
                  && _context.LeftGrabObject ==null && _context.RightGrabObject == null)
            {
                //������ ��� ���·� ����
                _context.IsItemGrabbing = true;
                if (IsItemGrabbing(_context.RightSearchTarget))
                    _sm.ChangeState(_sm.StateMap[PlayerState.EquipItem]);
                else
                    _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);

                return;
            }
            else
                _context.IsItemGrabbing = false;
        }
        else//�������� �ƴҶ� Ȥ�� ���� �ָ� ������
        {
            if (_context.LeftSearchTarget != null && _context.LeftGrabObject ==null)
            {
                _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);
                //���� ������� ����䰡 ������ id�� �����ϰ� �� ĳ���͵鿡�� JointFix�� ��Ŵ
                if (HandCollisionCheck(Side.Left))
                {
                    _sm.InputHandler.EnqueueCommand(COMMAND_KEY.FixJoint);
                    _grabDelayTimer = 0.5f;
                }
            }

            if (_context.RightSearchTarget != null && _context.RightGrabObject == null)
            {
                //�� ���� Action ����
                _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);

                if (HandCollisionCheck(Side.Right))
                {
                    _sm.InputHandler.EnqueueCommand(COMMAND_KEY.FixJoint);
                    _grabDelayTimer = 0.5f;
                }
            }
        }
    }



    //���߿� ����
    bool HandCollisionCheck(Define.Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (_sm.LeftHandCheckter.CollisionObject != null && //�պκ� nullüũ ������ �� �� ����
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


    bool IsHoldingItem(InteractableObject item, Define.Side side)
    {
        //HandChecker ��ũ��Ʈ���� ��� �� �������� �����̿� ���������� ����
        if (HandCollisionCheck(side))
        {
            _grabDelayTimer = 0.5f;
            _sm.Context.EquipItem = item;
            return true;
        }
        return false;
    }


    Side ItemDirCheck(Item item)
    {
        //�����հ� ������ ��ġ üũ�ؼ� ������ ���� ����
        Vector3 toItem = (item.TwoHandedPos.position - _context.Position).normalized; // �÷��̾ �������� �ٶ󺸴� ����
        Vector3 toOneHandedHandle = (item.OneHandedPos.position - _context.Position).normalized; // �������� ��ƾ��� oneHanded ������ ����
        Vector3 crossProduct = Vector3.Cross(toItem, toOneHandedHandle);

        if (crossProduct.y > 0)
            return Side.Right;// ���ڵ� �����̰� ������
        else
            return Side.Left;// ���ڵ� �����̰� ����
    }

}
