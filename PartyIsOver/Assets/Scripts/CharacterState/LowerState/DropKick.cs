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
        //���� ������
        if (!_sm.PlayerContext.IsLowerActionProgress)
        {
            _sm.ChangeState(_sm.IdleState);
        }
        else
        {
            //�̰� ����� ������ �� ���ǿ��� �̲��������� �۵�, ������Ű�� Action�ϳ��� �� �߰��ϴ� ������ ��ü ����
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
