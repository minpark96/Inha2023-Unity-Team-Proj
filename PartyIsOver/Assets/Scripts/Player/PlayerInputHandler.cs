using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Define;
using UnityEditor;

//플레이어가 할 수 있는 커맨드를 추가하고 키보드 키와 매핑하는 클래스

public class PlayerInputHandler : MonoBehaviourPun
{
    private Dictionary<COMMAND_KEY, CommandKey> _commands = new Dictionary<COMMAND_KEY, CommandKey>();
    private Define.COMMAND_KEY _activeCommandFlag;

    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;

    //작업공간이 바뀔때마다 유니티의 InputManager Axes를 설정할 필요없게 코드에서 처리하는 함수
    public void SetupInputAxes()
    {
        PlayerPrefs.SetString(COMMAND_KEY.LeftBtn.ToString(), "mouse 0");
        PlayerPrefs.SetString(COMMAND_KEY.RightBtn.ToString(), "mouse 1");
        PlayerPrefs.SetString(COMMAND_KEY.HeadButt.ToString(), "mouse 2");
        PlayerPrefs.SetString(COMMAND_KEY.Jump.ToString(), "space");
        PlayerPrefs.SetString(COMMAND_KEY.Skill.ToString(), "r");
        PlayerPrefs.SetString(COMMAND_KEY.ToggleRun.ToString(), "left shift");

        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        AddVirtualAxis(axesProperty, COMMAND_KEY.LeftBtn.ToString(), PlayerPrefs.GetString(COMMAND_KEY.LeftBtn.ToString()));
        AddVirtualAxis(axesProperty, COMMAND_KEY.RightBtn.ToString(), PlayerPrefs.GetString(COMMAND_KEY.RightBtn.ToString()));
        AddVirtualAxis(axesProperty, COMMAND_KEY.HeadButt.ToString(), PlayerPrefs.GetString(COMMAND_KEY.HeadButt.ToString()));
        AddVirtualAxis(axesProperty, COMMAND_KEY.Jump.ToString(), PlayerPrefs.GetString(COMMAND_KEY.Jump.ToString()));
        AddVirtualAxis(axesProperty, COMMAND_KEY.Skill.ToString(), PlayerPrefs.GetString(COMMAND_KEY.Skill.ToString()));
        AddVirtualAxis(axesProperty, COMMAND_KEY.ToggleRun.ToString(), PlayerPrefs.GetString(COMMAND_KEY.ToggleRun.ToString()));

        serializedObject.ApplyModifiedProperties();
    }

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
        _activeCommandFlag |= commandKey;
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

}