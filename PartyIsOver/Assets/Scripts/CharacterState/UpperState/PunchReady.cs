using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PunchReady : BodyState
{
    private UpperBodySM _sm;
    private float _pressDuration;
    private float _punchGrabThreshold = 0.2f;

    public PunchReady(StateMachine stateMachine) : base(PlayerState.PunchAndGrabReady, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _pressDuration = 0f;
    }

    public override void UpdateLogic()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _pressDuration += Time.deltaTime;

            if( _pressDuration > _punchGrabThreshold )
            {
                //�׷����·�
                _sm.ChangeState(_sm.StateMap[PlayerState.Grabbing]);
            }
        }
        else if (_pressDuration < _punchGrabThreshold)
        {
            //��ġ
            if (_sm.InputHandler.InputCommnadKey(COMMAND_KEY.LeftBtn, GetKeyType.Up))
            {
                _sm.ChangeState(_sm.StateMap[PlayerState.Punch]);
            }
        }

        //�ൿƮ���� ���÷��̿��� �� �κ��� �����ϰ� �ٷ� Punch�� Grabbing���� �Ѿ
        //�� PunchReady�� execute�� ���� ����
    }
    public override void GetInput()
    {
    }

    public override void Exit()
    {
    }
}
