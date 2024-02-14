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


        //�׷���Action ��� ����
        if (_grabDelayTimer < 0f)
            GrabbingFunc();

        //���� �տ� ���� �������� _sm�� ������ �־�� �ϰ� �װ� �Ǵ��ϴ� �ش���·� �̵�
        //1.����������, ������, �÷��̾����� ��


    }
    public override void GetInput()
    {
        //���콺 ���� Idle��
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

        //�߰��� ������Ʈ�� ������ ����
        if (_sm.LeftSearchTarget == null && _sm.RightSearchTarget == null)
            return;

        _isGrabbingInProgress = true;

        //Ÿ���� ���鿡 �ְ� �������϶�
        if (_sm.LeftSearchTarget == _sm.RightSearchTarget && _sm.LeftSearchTarget.IsItem)
        {
            //���� �Ÿ� �̳��� ������ ����� ���������
            if (_sm.TargetingHandler.TargetDistance(_targetingHandler.FindClosestCollisionPoint(_sm.LeftSearchTarget.ColliderObject))<= 1f
                  && !_isRightGrab && !_isLeftGrab)
            {
                //������ ��� ���·� ����
                HandleItemGrabbing(_sm.LeftSearchTarget.ItemObject);
                return;
            }
        }
        else//�������� �ƴҶ� Ȥ�� ���� �ָ� ������
        {
            //�޼� ����
            if (_sm.LeftSearchTarget != null && !_isLeftGrab)
            {
                //�� ���� Action ����

                //���� ������� ����䰡 ������ id�� �����ϰ� �� ĳ���͵鿡�� JointFix�� ��Ŵ
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
                //�� ���� Action ����

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
        //Ÿ�ټ�ġ �±׼��� �����Ұ�
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

        //objViewID �� �׷�������Ʈ�� ID
        PhotonView pv = PhotonNetwork.GetPhotonView(objViewID);
        if (photonView.IsMine && pv != null && EquipItem != null)
        {
            int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            pv.TransferOwnership(playerID);
        }

        //���� ���� �� �Ϻ� ����
        if ((Define.Side)side == Define.Side.Left)
        {
            if (_sm.LeftSearchTarget == null)
            {
                Debug.Log("Fail LeftJointFix");
            }

            //���Ⱑ ����
            _sm.LeftGrabJoint = _leftHandRigid.AddComponent<FixedJoint>();


            _sm.LeftGrabJoint.connectedBody = _sm.LeftSearchTarget.RigidbodyObject;
            //_grabJointLeft.breakForce = 9999999;

            //if (pv != null)
            //    _leftSearchTarget = pv.transform.GetComponent<InteractableObject>();


            if (_sm.LeftSearchTarget != null)
                _sm.LeftGrabObject = _sm.LeftSearchTarget;

            //�׼����� ����
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

            //���Ⱑ ����
            _sm.RightGrabJoint = _rightHandRigid.AddComponent<FixedJoint>();

            //if (pv != null)
            //    _rightSearchTarget = pv.transform.GetComponent<InteractableObject>();

            _sm.RightGrabJoint.connectedBody = _sm.RightSearchTarget.RigidbodyObject;
            //_grabJointRight.breakForce = 9001;

            if (_sm.RightSearchTarget != null)
                _sm.RightGrabObject = _sm.RightSearchTarget;

            //�׼����� ����
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
                    //�����ۿ� �°� �������� �Լ� �߰��ؾ���

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
                    //�����ۿ� �°� �������� �Լ� �߰��ؾ���

                    int rightObjViewID = _rightSearchTarget.transform.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("JointFix", RpcTarget.All, (int)Side.Right, rightObjViewID);
                }
                break;
        }
    }
}
