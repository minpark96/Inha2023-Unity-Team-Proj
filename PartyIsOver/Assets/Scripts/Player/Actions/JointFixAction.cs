using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 손으로 무언가를 잡을때 실행되는 관절 생성 액션
// 잡으려는 오브젝트와 손 사이에 관절 컴포넌트를 생성하여 연결시킨다.
public class JointFixAction : BaseAction
{
    public JointFixAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }
    
    BodyHandler _bodyHandler;
    PlayerActionContext _context;
    ItemType _type = ItemType.None;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _bodyHandler = bodyHandler;
        _context = data;

        // 아이템 장착의 경우 아이템의 레이어와 소유권 변경
        if (_context.EquipItem != null)
            ItemEquip();
        
        // 왼손 잡기
        if (_context.LeftSearchTarget != null && _context.LeftGrabJoint == null)
            JointFix((int)Define.Side.Left);

        // 오른손 잡기
        if (_context.RightSearchTarget != null && _context.RightGrabJoint == null)
            JointFix((int)Define.Side.Right);

        return true;
    }

    // side로 들어온 손과 해당 손이 잡으려는 오브젝트 사이에 관절을 생성한다.
    void JointFix(int side)
    {
        //관절 생성 및 연결
        if ((Define.Side)side == Define.Side.Left)
        {
            _context.LeftGrabObject = _context.LeftSearchTarget;
            _context.LeftGrabJoint = _bodyHandler.LeftHand.gameObject.AddComponent<FixedJoint>();
            _context.LeftGrabJoint.connectedBody = _context.LeftSearchTarget.RigidbodyObject;
        }
        else if ((Define.Side)side == Define.Side.Right)
        {
            _context.RightGrabObject = _context.RightSearchTarget;
            _context.RightGrabJoint = _bodyHandler.RightHand.gameObject.AddComponent<FixedJoint>();
            _context.RightGrabJoint.connectedBody = _context.RightSearchTarget.RigidbodyObject;
        }

        // 아이템을 장착한 경우 잡은 상태로 팔의 모션을 고정
        if (_context.EquipItem != null && (_type == ItemType.TwoHanded || _type == ItemType.Ranged))
            _bodyHandler.JointLock((Define.Side)side);
    }


    // 아이템을 장착할때 세부사항을 세팅해주는 함수
    void ItemEquip()
    {
        _type = _context.EquipItem.ItemObject.ItemData.ItemType;

        // 원거리 무기나 양손 무기일 경우 아이템을 적절하게 회전
        if(_type == ItemType.TwoHanded || _type == ItemType.Ranged)
        {
            if (_context.ItemHandleSide == Define.Side.Right)
                ItemRotate(_context.EquipItem, true);
            else
                ItemRotate(_context.EquipItem, false);

            // 잡은 상태로 팔의 모션을 고정
            CoroutineHelper.StartCoroutine(_bodyHandler.LockArmPosition());
        }

        // 아이템 레이어를 플레이어 자신의 레이어로 변경
        _context.EquipItem.gameObject.layer = _bodyHandler.gameObject.layer;
        
        // 아이템의 오너를 자신으로 변경하고, 무게를 가볍게 조정 
        _context.EquipItem.ItemObject.Owner = _bodyHandler.GetComponent<Actor>();
        _context.EquipItem.RigidbodyObject.mass = 0.3f;

        // 아이템의 네트워크상 소유권을 자기 자신으로 이전
        if (_context.IsMine && _context.EquipItem.PhotonView != null)
        {
            int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            _context.EquipItem.PhotonView.TransferOwnership(playerID);
        }
    }

    // 아이템 장착시 아이템을 적절하게 회전하는 함수
    void ItemRotate(InteractableObject item, bool isHandleRight)
    {
        Vector3 targetPosition = _bodyHandler.Chest.transform.forward;

        switch (item.ItemObject.ItemData.ItemType)
        {
            case ItemType.TwoHanded:
                //아이템의 헤드부분이 해당 방향벡터를 바라보게
                if (isHandleRight)
                    targetPosition = -_bodyHandler.Chest.transform.right;
                else
                    targetPosition = _bodyHandler.Chest.transform.right;
                break;
            case ItemType.Ranged:
                {
                    int id = 0;
                    if (item.PhotonView != null)
                        id = item.PhotonView.ViewID;
                    //photonView.RPC("ChangeWeaponSkin", RpcTarget.All, id);
                    targetPosition = -_bodyHandler.Chest.transform.up;
                }
                break;
            case ItemType.Consumable:
                targetPosition = _bodyHandler.Chest.transform.forward;
                break;
        }

        if (item.GetComponent<PhotonView>() != null)
        {
            int itemViewID = item.GetComponent<PhotonView>().ViewID;
            //photonView.RPC("SyncGrapItemPosition", RpcTarget.All, targetPosition, itemViewID);
            item.transform.right = -targetPosition.normalized;
            //EquipItem = PhotonNetwork.GetPhotonView(itemViewID).gameObject;
        }
    }
}
