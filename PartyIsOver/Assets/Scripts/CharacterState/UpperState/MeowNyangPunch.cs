using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeowNyangPunch : BodyState
{
    protected UpperBodySM _sm;

    public MeowNyangPunch(StateMachine stateMachine) : base("SkillState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        _sm.IsUpperActionProgress = true;
        //������ �Ӽ��� ���⼭ �ٲ�� �ϴ��� ����ؾ���
        //����, ����Ʈ�� ���⼭ �����ؾ� �ϴ��� ����ؾ���
        //_sm���� ����Ÿ���� �˷����� �׸��� DynamicData�� �ش� Ÿ���� ����
    }
    public override void UpdateLogic()
    {
        if (!_sm.IsUpperActionProgress)
        {
            _sm.ChangeState(_sm.IdleState);
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
