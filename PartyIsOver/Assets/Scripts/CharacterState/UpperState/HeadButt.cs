using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 박치기 상태
public class HeadButt : BaseState
{
    protected UpperBodySM _sm;

    public HeadButt(StateMachine stateMachine) : base(PlayerState.HeadButt, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        _sm.PlayerContext.IsUpperActionProgress = true;
    }
    // IsUpperActionProgress가 끝나면 박치기가 종료된것으로 판단하고 Idle상태로 변경
    public override void UpdateLogic()
    {
        if (!_sm.PlayerContext.IsUpperActionProgress)
        {
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
    }
    public override void GetInput()
    {
    }

    public override void UpdatePhysics()
    {
    }

    public override void Exit()
    {
    }
}
