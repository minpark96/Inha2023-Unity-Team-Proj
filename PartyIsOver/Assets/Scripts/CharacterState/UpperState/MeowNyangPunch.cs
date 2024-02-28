using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MeowNyangPunch : BodyState
{
    protected UpperBodySM _sm;

    public MeowNyangPunch(StateMachine stateMachine) : base(PlayerState.Skill, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        _sm.Context.IsUpperActionProgress = true;
        //������ �Ӽ��� ���⼭ �ٲ�� �ϴ��� ����ؾ���
        //����, ����Ʈ�� ���⼭ �����ؾ� �ϴ��� ����ؾ���
        //_sm���� ����Ÿ���� �˷����� �׸��� DynamicData�� �ش� Ÿ���� ����
    }
    public override void UpdateLogic()
    {
        if (!_sm.Context.IsUpperActionProgress)
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
