using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class UpperBodySM : StateMachine
{
    public PlayerContext PlayerContext;

    public Define.Side ReadySide = Define.Side.Left;
    public bool IsUpperActionProgress = false;
    public bool IsMeowPunch = false;
    public bool IsEquipItem = false;
    public Vector3 RightTargetDir = Vector3.zero;
    public Vector3 LeftTargetDir = Vector3.zero;

    public TargetingHandler TargetingHandler;
    public InteractableObject LeftSearchTarget = null;
    public InteractableObject RightSearchTarget = null;
    public InteractableObject LeftGrabObject = null;
    public InteractableObject RightGrabObject = null;
    public InteractableObject EquipItem;

    public FixedJoint LeftGrabJoint;
    public FixedJoint RightGrabJoint;

    public HandChecker LeftHandCheckter;
    public HandChecker RightHandCheckter;

    public IBaseState IdleState;
    public IBaseState PunchReadyState;
    public IBaseState PunchState;
    public IBaseState GrabbingState;
    public IBaseState SkillReadyState;
    public IBaseState SkillState;
    public IBaseState HeadButtState;


    public UpperBodySM(PlayerInputHandler inputHandler, TargetingHandler targetingHandler,PlayerContext playerContext)
    {
        IdleState = new UpperIdle(this);
        PunchReadyState = new PunchReady(this);
        PunchState = new Punch(this);
        GrabbingState = new Grabbing(this);
        SkillReadyState = new SkillReady(this);
        SkillState = new NuclearPunch(this);
        HeadButtState = new HeadButt(this);

        PlayerContext = playerContext;
        InputHandler = inputHandler;
        TargetingHandler = targetingHandler;
        
        //ChestTransform = chest;
        Init();

    }


    protected override IBaseState GetInitialState()
    {
        return IdleState;
    }
}
