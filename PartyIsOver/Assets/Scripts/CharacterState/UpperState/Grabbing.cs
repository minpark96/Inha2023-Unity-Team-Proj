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

    public Grabbing(StateMachine stateMachine) : base("GrabbingState", stateMachine)
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


        //�׷���Action ��� ����
        if (_grabDelayTimer < 0f)
            GrabbingProgress();

        //���� �տ� ���� �������� _sm�� ������ �־�� �ϰ� �װ� �Ǵ��ϴ� �ش���·� �̵�
        //1.����������, ������, �÷��̾����� ��


    }
    public override void GetInput()
    {
        //���콺 ���� Idle��
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            _sm.InputHandler.EnqueueCommand(COMMAND_KEY.DestroyJoint);
            _sm.ChangeState(_sm.IdleState);
        }
           
    }

    public override void UpdatePhysics()
    {
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
        if (_context.LeftSearchTarget == _context.RightSearchTarget && _context.LeftSearchTarget.ItemObject !=null)
        {
            //���� �Ÿ� �ȿ��� ����� ���������
            if (TargetingHandler.TargetDistance(TargetingHandler.FindClosestCollisionPoint
                (_context.Position,_context.LeftSearchTarget.ColliderObject,_context.Layer))<= 1f
                  && _context.LeftGrabObject ==null && _context.RightGrabObject == null)
            {
                //������ ��� ���·� ����
                //HandleItemGrabbing(_context.LeftSearchTarget.ItemObject);
                return;
            }
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

    //bool IsHoldingItem(Item item, Define.Side side)
    //{
    //    //HandChecker ��ũ��Ʈ���� ��� �� �������� �����̿� ���������� ����
    //    if (HandCollisionCheck(side))
    //    {
    //        _sm.Context.EquipItem.ItemObject = item;
    //        Debug.Log(_sm.Context.EquipItem);
    //        int id = _context.EquipItem.PhotonView.ViewID;
    //        photonView.RPC("UsingItemSetting", RpcTarget.All, id);
    //        return true;
    //    }
    //    return false;
    //}

    //void HandleItemGrabbing(Item item)
    //{
    //    switch (item.ItemData.ItemType)
    //    {
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
    //        case ItemType.Consumable:
    //            {
    //                Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
    //                _rightHandRigid.AddForce(dir.normalized * 80f);

    //                if (IsHoldingItem(item, Side.Right))
    //                    ItemRotate(item.transform, false);
    //                else
    //                    return;

    //                int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //                photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
    //            }
    //            break;
    //    }
    //}

    //void TwoHandedGrab(Item item)
    //{
    //    //������ ������� ������ �����̸� ���������� ��� ����
    //    if (ItemDirCheck(item))
    //    {
    //        Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
    //        _rightHandRigid.AddForce(dir.normalized * 90f);

    //        dir = item.TwoHandedPos.position - _leftHandRigid.transform.position;
    //        _leftHandRigid.AddForce(dir.normalized * 90f);

    //        if (IsHoldingItem(item, Define.Side.Both))
    //            ItemRotate(item.transform, true);
    //        else
    //            return;
    //    }
    //    else
    //    {
    //        Vector3 dir = item.TwoHandedPos.position - _rightHandRigid.transform.position;
    //        _rightHandRigid.AddForce(dir.normalized * 90f);

    //        dir = item.OneHandedPos.position - _leftHandRigid.transform.position;
    //        _leftHandRigid.AddForce(dir.normalized * 90f);

    //        if (IsHoldingItem(item, Define.Side.Both))
    //            ItemRotate(item.transform, false);
    //        else
    //            return;
    //    }

    //    int leftObjViewID = _leftSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //    photonView.RPC("JointFix", RpcTarget.All, (int)Define.Side.Left, leftObjViewID);
    //    int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //    photonView.RPC("JointFix", RpcTarget.All, (int)Define.Side.Right, rightObjViewID);

    //    photonView.RPC("LockArmTrigger", RpcTarget.All);

    //}
}
