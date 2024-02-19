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

    public EquipItem(StateMachine stateMachine) : base("EquipItemState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _item = _sm.Context.EquipItem.ItemObject;
        _itemCoolTime = _item.ItemData.CoolTime;
    }

    public override void UpdateLogic()
    {
        _coolTimeTimer -= Time.deltaTime;
    }

    public override void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _coolTimeTimer < 0f)
        {
            _coolTimeTimer = _itemCoolTime;
            //�����ۻ�� Ŀ�ǵ�
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _sm.ChangeState(_sm.IdleState);
            //���Ÿ� ������ ��� ��ô
        }
    }

    public override void Exit()
    {
        //�׷����� Ŀ�ǵ�
    }
}
