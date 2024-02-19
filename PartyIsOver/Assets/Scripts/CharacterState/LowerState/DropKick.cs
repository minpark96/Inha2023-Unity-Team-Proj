using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKick : BodyState
{
    private LowerBodySM _sm;

    public DropKick(StateMachine stateMachine) : base("DropKickState", stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        _sm.IsLowerActionProgress = true;
    }

    public override void UpdateLogic()
    {
        //���� ������
        if (!_sm.IsLowerActionProgress)
        {
            _sm.ChangeState(_sm.IdleState);
        }
        else
        {
            //�̰� ����� ������ �� ���ǿ��� �̲��������� �۵�, ������Ű�� Action�ϳ��� �� �߰��ϴ� ������ ��ü ����
            _sm.InputHandler.EnqueueCommand(Define.COMMAND_KEY.Move);
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