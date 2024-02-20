using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EquipItem : BodyState
{
    private UpperBodySM _sm;
    private Item _item;
    private float _itemCoolTime;
    private float _coolTimeTimer = 0f;
    private bool _exitFlag;
    public EquipItem(StateMachine stateMachine) : base(PlayerState.EquipItem, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _item = _sm.Context.EquipItem.ItemObject;
        _itemCoolTime = _item.ItemData.CoolTime;
        _exitFlag = false;
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.FixJoint);

        if (_item.ItemData.ItemType == ItemType.Ranged)
            ChangeWeaponSkin();
    }

    public override void UpdateLogic()
    {
        _coolTimeTimer -= Time.deltaTime;
    }

    public override void GetInput()
    {
        if (_sm.InputHandler.InputCommnadKey(KeyCode.Mouse0, GetKeyType.Down) && _coolTimeTimer < 0f)
        {
            _coolTimeTimer = _itemCoolTime;
            if (_item.ItemData.ItemType == ItemType.Consumable)
                _exitFlag = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //원거리 무기의 경우 투척
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
    }

    public override void Exit()
    {
        _sm.RangeWeaponSkin.gameObject.SetActive(false);
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.DestroyJoint);
    }

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
