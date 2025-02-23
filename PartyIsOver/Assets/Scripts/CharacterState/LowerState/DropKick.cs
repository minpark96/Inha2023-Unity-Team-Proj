using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드롭킥 상태
public class DropKick : BaseState
{
    private LowerBodySM _sm;

    public DropKick(StateMachine stateMachine) : base(Define.PlayerState.DropKick, stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        // IsLowerActionProgress를 켜서 true인 동안 다른 동작으로 넘어가지 않게끔 한다.
        _sm.PlayerContext.IsLowerActionProgress = true;
    }

    public override void UpdateLogic()
    {
        //동작이 종료되어 IsLowerActionProgress가 false가 되면 상태 나가기
        if (!_sm.PlayerContext.IsLowerActionProgress)
        {
            _sm.ChangeState(_sm.IdleState);
        }
        else
        {
            //이거 지우면 발차기 후 빙판에서 미끄러지듯이 작동, 마찰시키는 Action하나를 더 추가하는 식으로 대체 가능
            InvokeReserveCommand(Define.COMMAND_KEY.Move);
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
