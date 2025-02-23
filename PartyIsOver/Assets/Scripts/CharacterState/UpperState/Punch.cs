using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 펀치 상태
public class Punch : BaseState
{
    private UpperBodySM _sm;

    public Punch(StateMachine stateMachine) : base(PlayerState.Punch, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        // 펀치 시작과 동시에 IsUpperActionProgress를 켜서 다른 동작으로 넘어가지 않게끔 한다.
        _sm.PlayerContext.IsUpperActionProgress = true;
        
        //데미지 속성을 여기서 바꿔야 하는지 고민해야함
        //사운드, 이펙트를 여기서 관리해야 하는지 고민해야함
        //_sm에게 공격타입을 알려야함 그리고 DynamicData가 해당 타입을 저장
    }

    public override void UpdateLogic()
    {
        // IsUpperActionProgress가 끝나면 Idle 상태로 변경
        if(!_sm.PlayerContext.IsUpperActionProgress)
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
    
    // 펀치 한번이 끝날 때 마다 준비된 손의 좌우 방향을 바꿔준다. (양 손을 번갈아 펀치하기 위함)
    public override void Exit()
    {
        if (_sm.ReadySide == Side.Left)
            _sm.ReadySide = Side.Right;
        else
            _sm.ReadySide = Side.Left;
    }
}
