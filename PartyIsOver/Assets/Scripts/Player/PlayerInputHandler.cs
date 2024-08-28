using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Define;
#if UNITY_EDITOR
using UnityEditor;
#endif
//�÷��̾ �� �� �ִ� Ŀ�ǵ带 �߰��ϰ� Ű���� Ű�� �����ϴ� Ŭ����

public class PlayerInputHandler : MonoBehaviourPun
{
    private Dictionary<COMMAND_KEY, CommandKey> _commands = new Dictionary<COMMAND_KEY, CommandKey>();
    private Define.COMMAND_KEY _activeCommandFlag;

    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;

#if UNITY_EDITOR
    //�۾������� �ٲ𶧸��� ����Ƽ�� InputManager Axis�� ������ �ʿ���� �ڵ忡�� ó���ϴ� �Լ�
    public void SetupInputAxes()
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        //Define���� ������ COMMAND_KEY enum�� �̸����� Axis�� �����ϰ� Positive���� CommandBtnName�� ����
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


    //Actor���� Ŀ�ǵ带 �߰�
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

    //�÷��̾��� ��������� ����
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

    //�÷��׿� Ŀ�ǵ� ����
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

    //����� Ŀ�ǵ带 Actor���� �����ִ� ����
    public COMMAND_KEY GetActiveCmdFlag()
    {
        return _activeCommandFlag;
    }

    //Update ����Ŭ���� ����� Ŀ�ǵ尡 FixedUpdate���� ó��������� ����Ǵ� �Լ�
    public void ClearCommand()
    {
        _activeCommandFlag = 0f;
    }



}