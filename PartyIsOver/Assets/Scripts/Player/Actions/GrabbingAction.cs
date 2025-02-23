using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// 잡기 액션
// 벽, 박스, 아이템, 다른 플레이어등을 향해 팔을 뻗는 동작

// 팔만 뻗고 이 클래스에서 다른건 처리하지 않는다.
//(이 클래스에선 팔만 뻗고 다른 오브젝트가 손에 닿았는지 등은 상체 상태머신의 Grabbing 상태의
// FixedUpdate에서 HandCollisionCheck가 계속 실행되어 손이 서치한 오브젝트에 닿았는지 계속 체크한다.
// 닿았으면 상태를 벽타기, 박스 들기 등으로 변경하거나 접촉한 아이템을 장착한다.)

public class GrabbingAction:BaseAction
{
    public GrabbingAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }
    PlayerActionContext _context;
    Rigidbody _leftHandRigid;
    Rigidbody _rightHandRigid;


    bool _isGrounded;
    Vector3 _leftAddForceDir;
    Vector3 _rightAddForceDir;
    Vector3 _leftTargetDir;
    Vector3 _rightTargetDir;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _context = data;
        _isGrounded = data.IsGrounded;

        _leftTargetDir = data.LeftTargetDir;
        _rightTargetDir = data.RightTargetDir;
        _leftHandRigid = bodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = bodyHandler.RightHand.PartRigidbody;
        
        // 서치된 타겟이 아이템일 경우
        if(_context.IsItemGrabbing && _context.RightSearchTarget != null && _context.RightSearchTarget.ItemObject !=null)
        {
            Debug.Log(_context.RightSearchTarget);
            ItemDirSetting(_context.RightSearchTarget.ItemObject);
        }
        else // 서치된 타겟이 아이템이 아닐 경우
            NonItemDirSetting();

        Grabbing();
        return true;
    }

    // _leftAddForceDir 변수에 손이 나아갈 방향을 저장하고 해당 방향으로 손을 이동
    // 
    private void Grabbing()
    {
        if (_leftTargetDir != Vector3.zero)
            _leftHandRigid.AddForce(_leftAddForceDir.normalized * 150f);

        if (_rightTargetDir != Vector3.zero)
            _rightHandRigid.AddForce(_rightAddForceDir.normalized * 150f);

    }
    
    // 플레이어가 아이템이 아닌 대상(벽, 박스, 다른 플레이어)를 잡을 때 손의 이동 방향을 계산하는 함수
    private void NonItemDirSetting()
    {
        if (_leftTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                _leftAddForceDir = ((_leftTargetDir + Vector3.up * 2) - _leftHandRigid.transform.position).normalized;
            else
                _leftAddForceDir = (_leftTargetDir - _leftHandRigid.transform.position).normalized;
        }

        if (_rightTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                _rightAddForceDir = ((_rightTargetDir + Vector3.up * 2) - _rightHandRigid.transform.position).normalized;
            else
                _rightAddForceDir = (_rightTargetDir - _rightHandRigid.transform.position).normalized;
        }
    }
    
    // 아이템의 종류에 따라 잡기 방법을 다르게 적용
    private void ItemDirSetting(Item item)
    {
        switch (item.ItemData.ItemType)
        {
            case ItemType.TwoHanded:
                    TwoHandedGrab(item);
                break;
            case ItemType.Ranged:
                    TwoHandedGrab(item);
                break;
            case ItemType.Consumable:
                {
                    _rightAddForceDir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _leftTargetDir = Vector3.zero;
                    _rightTargetDir = Vector3.up;
                }
                break;
        }
    }

    // 양손 무기일 경우 손잡이 위치를 적절하게 조정하여 잡기
    void TwoHandedGrab(Item item)
    {
        Vector3 rightGripPos = item.TwoHandedPos.position;
        Vector3 leftGripPos = item.OneHandedPos.position;

        //아이템 방향따라 오른쪽 손잡이를 오른손으로 잡기 진행
        if (_context.ItemHandleSide == Side.Right)
        {
            rightGripPos = item.OneHandedPos.position;
            leftGripPos = item.TwoHandedPos.position;
        }

        _rightAddForceDir = rightGripPos - _rightHandRigid.transform.position;
        _leftAddForceDir = leftGripPos - _leftHandRigid.transform.position;
        _leftTargetDir = Vector3.up;
        _rightTargetDir = Vector3.up;
    }
}
