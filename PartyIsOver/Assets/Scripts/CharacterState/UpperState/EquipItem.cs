using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 아이템을 장착한 상태 
public class EquipItem : BaseState
{
    private UpperBodySM _sm;
    private Item _item;
    private float _itemCoolTime;
    private float _coolTimeTimer = 0f;



    public EquipItem(StateMachine stateMachine) : base(PlayerState.EquipItem, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _item = _sm.PlayerContext.EquipItem.ItemObject;
        _itemCoolTime = _item.ItemData.CoolTime;
        _sm.PlayerContext.IsUpperActionProgress = true;
        // 상태 진입과 동시에 관절 생성 커맨드 예약
        InvokeReserveCommand(COMMAND_KEY.FixJoint);
        
        // 원거리 무기일 경우 해당 오브젝트를 숨기고 스킨을 입힘
        if (_item.ItemData.ItemType == ItemType.Ranged)
            ChangeWeaponSkin();
    }

    public override void UpdateLogic()
    {
        _coolTimeTimer -= Time.deltaTime;
        
        // 이건 여기 있어야 하는지 고민해봐야함
        // 액션이 종료되어 IsUpperActionProgress가 false가 되면 Idle 상태로 변경
        if(!_sm.PlayerContext.IsUpperActionProgress)
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
    }

    public override void GetInput()
    {
        // 왼쪽 클릭 입력이 들어올 경우
        if(InputCommand(COMMAND_KEY.LeftBtn, KeyType.Down) && _coolTimeTimer < 0f)
        {
            // 왼쪽 클릭에 해당하는 커맨드를 예약(ItemUse)
            InvokeReserveCommand(COMMAND_KEY.LeftBtn);
            _coolTimeTimer = _itemCoolTime;
        }
        
        // 오른쪽 클릭 입력이 들어올 경우
        if (InputCommand(COMMAND_KEY.RightBtn, KeyType.Down))
        {
            // 오른쪽 클릭에 해당하는 커맨드를 예약(Throw)
            InvokeReserveCommand(COMMAND_KEY.RightBtn);
            if (_item.ItemData.ItemType != ItemType.Consumable)
                _sm.PlayerContext.IsUpperActionProgress = false;
        }
    }

    public override void Exit()
    {
        _sm.RangeWeaponSkin.gameObject.SetActive(false);
        InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
    }
    
    // 총기가 손잡이로 충돌해서 잡을 경우 정면을 보지 않기 때문에 총기 스킨을 입혀서 정면을 보게 만드는 함수
    private void ChangeWeaponSkin()
    {
        _sm.RangeWeaponSkin.gameObject.SetActive(true);

        //RangeWeapon item = PhotonNetwork.GetPhotonView(id).transform.GetComponent<RangeWeapon>();
        Define.RangeWeapon weapon = Define.RangeWeapon.IceGun;
        _item.Body.gameObject.SetActive(false);

        switch (_item.ItemData.UseDamageType)
        {
            case InteractableObject.Damage.Ice:
                    weapon = Define.RangeWeapon.IceGun;
                break;
            case InteractableObject.Damage.Shock:
                    weapon = Define.RangeWeapon.StunGun;
                break;
        }

        for (int i = 0; i < _sm.RangeWeaponSkin.childCount; i++)
            _sm.RangeWeaponSkin.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);

        _sm.RangeWeaponSkin.GetChild(0).GetChild(0).GetChild((int)weapon).gameObject.SetActive(true);
        _sm.FirePoint = _sm.RangeWeaponSkin.GetChild(0).GetChild(1);
    }
}
