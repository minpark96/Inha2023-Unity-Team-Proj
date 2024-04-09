using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKick : BaseState
{
    private LowerBodySM _sm;

    public DropKick(StateMachine stateMachine) : base(Define.PlayerState.DropKick, stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        _sm.PlayerContext.IsLowerActionProgress = true;
    }

    public override void UpdateLogic()
    {
        //상태 나가기
        if (!_sm.PlayerContext.IsLowerActionProgress)
        {
            _sm.ChangeState(_sm.IdleState);
        }
        else
        {
            //이거 지우면 발차기 후 빙판에서 미끄러지듯이 작동, 마찰시키는 Action하나를 더 추가하는 식으로 대체 가능
            _sm.InputHandler.ReserveCommand(Define.COMMAND_KEY.Move);
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
