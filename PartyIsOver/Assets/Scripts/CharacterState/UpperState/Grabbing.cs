using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbing : BodyState
{
    private UpperBodySM _sm;


    public Grabbing(StateMachine stateMachine) : base("GrabbingState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
    }

    public override void UpdateLogic()
    {
        //그래빙Action 계속 실행
        
        //각각 손에 뭔가 잡힌것을 _sm이 가지고 있어야 하고 그걸 판단하다 해당상태로 이동
        //1.아이템인지, 벽인지, 플레이어인지 등
        
        //
    }
    public override void GetInput()
    {
        //마우스 떼면 Idle로
    }

    public override void UpdatePhysics()
    {
    }

    public override void Exit()
    {
    }


}
