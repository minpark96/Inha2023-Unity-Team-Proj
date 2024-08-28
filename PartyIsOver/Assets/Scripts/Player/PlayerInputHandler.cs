using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Define;
#if UNITY_EDITOR
using UnityEditor;
#endif
//플레이어가 할 수 있는 커맨드를 추가하고 키보드 키와 매핑하는 클래스

public class PlayerInputHandler : MonoBehaviourPun
{
    private Dictionary<COMMAND_KEY, CommandKey> _commands = new Dictionary<COMMAND_KEY, CommandKey>();
    private Define.COMMAND_KEY _activeCommandFlag;

    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;

#if UNITY_EDITOR
    //작업공간이 바뀔때마다 유니티의 InputManager Axis를 설정할 필요없게 코드에서 처리하는 함수
    public void SetupInputAxes()
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        //Define에서 정의한 COMMAND_KEY enum의 이름으로 Axis를 생성하고 Positive값을 CommandBtnName로 저장
        AddVirtualAxis(axesProperty, COMMAND_KEY.LeftBtn.ToString(),   CommandBtnName.LeftBtn);
        AddVirtualAxis(axesProperty, COMMAND_KEY.RightBtn.ToString(),  CommandBtnName.RightBtn);
        AddVirtualAxis(axesProperty, COMMAND_KEY.HeadButt.ToString(),  CommandBtnName.HeadButt);
        AddVirtualAxis(axesProperty, COMMAND_KEY.Jump.ToString(),      CommandBtnName.Jump);
        AddVirtualAxis(axesProperty, COMMAND_KEY.Skill.ToString(),     CommandBtnName.Skill);
        AddVirtualAxis(axesProperty, COMMAND_KEY.ToggleRun.ToString(), CommandBtnName.ToggleRun);

        serializedObject.ApplyModifiedProperties();
    }
    void AddVirtualAxis(SerializedProperty axesProperty, string axisName, string positiveButton)
    {
        axesProperty.arraySize++;
        SerializedProperty axis = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

        axis.FindPropertyRelative("m_Name").stringValue = axisName;
        axis.FindPropertyRelative("positiveButton").stringValue = positiveButton;
        axis.FindPropertyRelative("negativeButton").stringValue = string.Empty;
        axis.FindPropertyRelative("altNegativeButton").stringValue = string.Empty;
        axis.FindPropertyRelative("gravity").floatValue = 1000f;
        axis.FindPropertyRelative("dead").floatValue = 0.001f;
        axis.FindPropertyRelative("sensitivity").floatValue = 1000f;
        axis.FindPropertyRelative("snap").boolValue = false;
        axis.FindPropertyRelative("invert").boolValue = false;
        axis.FindPropertyRelative("type").intValue = 0;
        axis.FindPropertyRelative("axis").intValue = 0;
        axis.FindPropertyRelative("joyNum").intValue = 0;
    }
#endif


    //Actor에게 커맨드를 추가
    public void InitCommand(Actor actor)
    {
        _commands.Add(COMMAND_KEY.Jump,         new CmdJump(actor));
        _commands.Add(COMMAND_KEY.Move,         new CmdMove(actor));
        _commands.Add(COMMAND_KEY.LeftBtn,      new CmdLeftBtn(actor));
        _commands.Add(COMMAND_KEY.Skill,        new CmdSkill(actor));
        _commands.Add(COMMAND_KEY.Charge,       new CmdCharge(actor));
        _commands.Add(COMMAND_KEY.ResetCharge,  new CmdResetCharge(actor));
        _commands.Add(COMMAND_KEY.HeadButt,     new CmdHeadButt(actor));
        _commands.Add(COMMAND_KEY.RightBtn,     new CmdRightBtn(actor));
        _commands.Add(COMMAND_KEY.Grabbing,     new CmdGrabbing(actor));
        _commands.Add(COMMAND_KEY.TargetSearch, new CmdSearchTarget(actor));
        _commands.Add(COMMAND_KEY.FixJoint,     new CmdFixJoint(actor));
        _commands.Add(COMMAND_KEY.DestroyJoint, new CmdDestroyJoint(actor));
    }

    //플레이어의 진행방향을 리턴
    public Vector3 GetMoveInput(Transform cameraArm)
    {
        _moveInput.x = Input.GetAxis("Horizontal");
        _moveInput.y = 0;
        _moveInput.z = Input.GetAxis("Vertical");

        _lookForward.x = cameraArm.forward.x;
        _lookForward.y = 0f;
        _lookForward.z = cameraArm.forward.z;
        _lookForward.Normalize();

        _lookRight.x = cameraArm.right.x;
        _lookRight.y = 0f;
        _lookRight.z = cameraArm.right.z;
        _lookRight.Normalize();

        _moveDir = _lookForward * _moveInput.z + _lookRight * _moveInput.x;
        return _moveDir;
    }

    //플래그에 커맨드 예약
    public void ReserveCommand(COMMAND_KEY commandKey)
    {
        photonView.RPC(nameof(ReserveCmdMaster), RpcTarget.MasterClient, (int)commandKey);
    }

    [PunRPC]
    private void ReserveCmdMaster(int commandKey)
    {
        _activeCommandFlag |= (COMMAND_KEY)commandKey;
    }

    public CommandKey GetCommand(COMMAND_KEY key)
    {
        return _commands[key];
    }

    //예약된 커맨드를 Actor에게 보내주는 역할
    public COMMAND_KEY GetActiveCmdFlag()
    {
        return _activeCommandFlag;
    }

    //Update 사이클동안 예약된 커맨드가 FixedUpdate에서 처리됐을경우 실행되는 함수
    public void ClearCommand()
    {
        _activeCommandFlag = 0f;
    }



}