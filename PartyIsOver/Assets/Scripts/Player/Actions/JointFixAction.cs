using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static PlayerController;

public class JointFixAction
{
    public JointFixAction(ActionController actions)
    {
        actions.OnJointFix -= HandleJointFixEvent;
        actions.OnJointFix += HandleJointFixEvent;
    }

    BodyHandler _bodyHandler;
    PlayerContext _context;
    ItemType _type = ItemType.None;

    bool HandleJointFixEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _bodyHandler = bodyHandler;
        _context = data;

        if (_context.LeftSearchTarget != null && _context.LeftGrabJoint == null)
            JointFix((int)Define.Side.Left);

        if (_context.RightSearchTarget != null && _context.RightGrabJoint == null)
            JointFix((int)Define.Side.Right);

        if(_context.EquipItem != null)
            ItemEquip();
        

        return true;
    }

    void JointFix(int side)
    {
        //아이템 장착시
        if (_context.EquipItem != null)
            _type = _context.EquipItem.ItemObject.ItemData.ItemType;
        

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

        //관절 고정
        if (_context.EquipItem != null && (_type == ItemType.TwoHanded || _type == ItemType.Ranged))
            _bodyHandler.JointLock((Define.Side)side);
        
    }


    void ItemEquip()
    {
        ItemType type = _context.EquipItem.ItemObject.ItemData.ItemType;

        //아이템 회전
        if(type == ItemType.TwoHanded || type == ItemType.Ranged)
        {
            if (_context.ItemHandleSide == Define.Side.Right)
                ItemRotate(_context.EquipItem, true);
            else
                ItemRotate(_context.EquipItem, false);

            CoroutineHelper.StartCoroutine(_bodyHandler.LockArmPosition());
        }

        //레이어 변경
        _context.EquipItem.gameObject.layer = _bodyHandler.gameObject.layer;

        _context.EquipItem.ItemObject.Owner = _bodyHandler.GetComponent<Actor>();
        _context.EquipItem.RigidbodyObject.mass = 0.3f;

        //owner변경
        if (_context.IsMine && _context.EquipItem.PhotonView != null)
        {
            int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            _context.EquipItem.PhotonView.TransferOwnership(playerID);
        }
    }

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

    //private void ChangeWeaponSkin(int id)
    //{
    //    RangeWeaponSkin.gameObject.SetActive(true);


    //    RangeWeapon item = PhotonNetwork.GetPhotonView(id).transform.GetComponent<RangeWeapon>();
    //    Define.RangeWeapon weapon = Define.RangeWeapon.IceGun;
    //    item.GetComponent<Item>().Body.gameObject.SetActive(false);


    //    switch (item.ItemData.UseDamageType)
    //    {
    //        case InteractableObject.Damage.Ice:
    //            {
    //                weapon = Define.RangeWeapon.IceGun;
    //            }
    //            break;
    //        case InteractableObject.Damage.Shock:
    //            {
    //                weapon = Define.RangeWeapon.StunGun;
    //            }
    //            break;
    //    }

    //    for (int i = 0; i < RangeWeaponSkin.childCount; i++)
    //        RangeWeaponSkin.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);


    //    RangeWeaponSkin.GetChild(0).GetChild(0).GetChild((int)weapon).gameObject.SetActive(true);
    //    FirePoint = RangeWeaponSkin.GetChild(0).GetChild(1);
    //}
}
