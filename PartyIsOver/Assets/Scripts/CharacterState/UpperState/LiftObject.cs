using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 오브젝트를 머리 위로 들어올리는 상태 (플레이어나, 박스와 같은 아이템이 아닌 오브젝트)
public class LiftObject : BaseState
{
    private UpperBodySM _sm;

    public LiftObject(StateMachine stateMachine) : base(PlayerState.LiftObject, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        InvokeReserveCommand(COMMAND_KEY.FixJoint);
    }

    public override void UpdateLogic()
    {
        InvokeReserveCommand(COMMAND_KEY.LeftBtn);
    }

    public override void GetInput()
    {
        // 들어올린 오브젝트를 버리는 액션
        if(!InputCommand(COMMAND_KEY.LeftBtn, KeyType.Press))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        
        // 들어올린 오브젝트를 정면으로 던지는 액션
        if(InputCommand(COMMAND_KEY.RightBtn, KeyType.Down))
        {
            InvokeReserveCommand(COMMAND_KEY.RightBtn);
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
    }
    
    // 상태 해제시 관절 제거
    public override void Exit()
    {
        InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
    }
}
