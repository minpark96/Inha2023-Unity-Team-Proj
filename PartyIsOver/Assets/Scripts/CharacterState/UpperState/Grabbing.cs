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


        //�׷���Action ��� ����
        if (_grabDelayTimer < 0f)
            GrabbingFunc();

        //���� �տ� ���� �������� _sm�� ������ �־�� �ϰ� �װ� �Ǵ��ϴ� �ش���·� �̵�
        //1.����������, ������, �÷��̾����� ��


    }
    public override void GetInput()
    {
        //���콺 ���� Idle��
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
        //��ġ�׼�
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.TargetSearch);

        //Debug.Log(_leftSearchTarget);
        //Debug.Log(_rightSearchTarget);

        //�߰��� ������Ʈ�� ������ ����
        if (_context.LeftSearchTarget == null && _context.LeftSearchTarget == null)
            return;

        _isGrabbingInProgress = true;

        //Ÿ���� ���鿡 �ְ� �������϶�
        if (_context.LeftSearchTarget == _context.LeftSearchTarget && _context.LeftSearchTarget.IsItem)
        {
            _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);
            //���� �Ÿ� �ȿ��� ����� ���������
            if (TargetingHandler.TargetDistance(TargetingHandler.FindClosestCollisionPoint
                (_context.Position,_context.LeftSearchTarget.ColliderObject,_context.Layer))<= 1f
                  && _context.LeftGrabObject ==null && _context.RightGrabObject != null)
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
                Debug.Log("gabbing44");
                _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Grabbing);

                //���� ������� ����䰡 ������ id�� �����ϰ� �� ĳ���͵鿡�� JointFix�� ��Ŵ
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
                //�� ���� Action ����
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
    //                //�����ۿ� �°� �������� �Լ� �߰��ؾ���

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
    //                //�����ۿ� �°� �������� �Լ� �߰��ؾ���

    //                int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
    //                photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
    //            }
    //            break;
    //    }
    //}
}
