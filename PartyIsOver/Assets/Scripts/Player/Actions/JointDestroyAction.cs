using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// 손으로 잡은걸 놓을 때 실행되는 관절 분리 액션
// 잡고 있는 물체와 연결된 관절 컴포넌트를 삭제하고 물체를 떨어뜨린다.

public class JointDestroyAction : BaseAction
{
    public JointDestroyAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    PlayerActionContext _context;
    BodyHandler _bodyHandler;
    Item _equipItem;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _context = data;
        _bodyHandler = bodyHandler;
        ResetGrab();
        return true;
    }

    // 손으로 관절을 연결해 잡고 있던 걸 관절을 해제하여 놓는 함수
    // photonview의 소유권 처리나 아이템을 손에서 놓아 소유권을 없는 상태로 되돌리는 작업도 같이 한다.
    void ResetGrab()
    {     
        // 현재 캐릭터가 로컬 클라이언트의 주 캐릭터면
        if(_context.IsMine)
        {
            // 플레이어가 잡고 있던 양손의 물체의 소유권은 마스터 클라이언트로 이전
            int PlayerID = PhotonNetwork.MasterClient.ActorNumber;
            if (_context.LeftGrabObject != null && _context.LeftGrabObject.PhotonView != null)
                _context.LeftGrabObject.PhotonView.TransferOwnership(PlayerID);
            if (_context.RightGrabObject != null && _context.RightGrabObject.PhotonView != null)
                _context.RightGrabObject.PhotonView.TransferOwnership(PlayerID);
        }
        if(_context.EquipItem != null) // 아이템 장착 상태일 경우
        {
            _equipItem = _context.EquipItem.ItemObject;
            
            // 아이템 레이어를 Item으로 변경하고, 아이템의 Body를 활성화
            _context.EquipItem.gameObject.layer = (int)Define.Layer.Item;
            _equipItem.Body.gameObject.SetActive(true);

            // 아이템의 소유자 정보를 null로 변경
            _context.EquipItem.ItemObject.Owner = null;
            
            // 아이템의 데미지 타입은 Default로 변경
            if (_equipItem.ItemData.ItemType == ItemType.OneHanded ||
                _equipItem.ItemData.ItemType == ItemType.TwoHanded)
                _context.EquipItem.damageModifier = InteractableObject.Damage.Default;
            
            // 아이템 무게 조정
            _context.EquipItem.RigidbodyObject.mass = 10f;
            
            // 컨텍스트의 장착된 아이템을 null로 변경
            _context.EquipItem = null;
        }

        // 관절을 삭제하고, 양 팔의 고정된 포지션을 해제
        _bodyHandler.DestroyJoint(_context.RightGrabJoint, _context.LeftGrabJoint);
        _bodyHandler.UnlockArmPosition();

        // 컨텍스트에서 양 손에 잡힌 오브젝트를 null로 변경
        _context.LeftGrabObject = null;
        _context.RightGrabObject = null;
        // _actor.GrabState = GrabState.None;
    }
}
