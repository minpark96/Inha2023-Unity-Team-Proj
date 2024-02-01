using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : BodyState
{
    private UpperBodySM _sm;

    public Punch(StateMachine stateMachine) : base("PunchState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        //������ �Ӽ��� ���⼭ �ٲ�� �ϴ��� ����ؾ���
        //����, ����Ʈ�� ���⼭ �����ؾ� �ϴ��� ����ؾ���
        //_sm���� ����Ÿ���� �˷����� �׸��� DynamicData�� �ش� Ÿ���� ����
    }

    public override void UpdateLogic()
    {
        //action �ڷ�ƾ���� ��ġ �׼��� ������ _sm���� �̺�Ʈ�� �׼��� �������� �˷�����
        //_sm�� attacking�� false�� �Ǹ� Idle�� ����
        if(!_sm.IsAttacking)
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
        if (_sm.ReadySide == Define.Side.Left)
            _sm.ReadySide = Define.Side.Right;
        else
            _sm.ReadySide = Define.Side.Left;
    }
}
