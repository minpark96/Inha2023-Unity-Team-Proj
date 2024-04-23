using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Define;
using UnityEditor;

public class PlayerInputHandler : MonoBehaviourPun
{
    private Dictionary<COMMAND_KEY, CommandKey> _commands = new Dictionary<COMMAND_KEY, CommandKey>();
    private Define.COMMAND_KEY _activeCommandFlag;

    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;


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

    public void ReserveCommand(COMMAND_KEY commandKey)
    {
        _activeCommandFlag |= commandKey;
    }

    public CommandKey GetCommand(COMMAND_KEY key)
    {
        return _commands[key];
    }

    public COMMAND_KEY GetActiveCmdFlag()
    {
        return _activeCommandFlag;
    }

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
    //private Dictionary<COMMAND_KEY,InputAction> _buttonList = new Dictionary<COMMAND_KEY, InputAction>();
    //private List<InputManagerEntry> _buttonList = new List<InputManagerEntry>();

    //public void InitButton()
    //{
    //    _buttonList.Add(AddButtonEntry(COMMAND_KEY.Jump.ToString(), "space"));
    //    _buttonList.Add(AddButtonEntry(COMMAND_KEY.LeftBtn.ToString(), "Mouse0"));
    //    _buttonList.Add(AddButtonEntry(COMMAND_KEY.RightBtn.ToString(), "Mouse1"));
    //    _buttonList.Add(AddButtonEntry(COMMAND_KEY.HeadButt.ToString(), "Mouse2"));
    //    _buttonList.Add(AddButtonEntry(COMMAND_KEY.Skill.ToString(), "r"));
    //    InputRegistering.RegisterInputs(_buttonList);


    //    _buttonList.Add(COMMAND_KEY.Move, new InputAction(COMMAND_KEY.Move.ToString()));
    //    _buttonList[COMMAND_KEY.Move].AddBinding("<Keyboard>/W");
    //    _buttonList[COMMAND_KEY.Move].AddBinding("<Keyboard>/A");
    //    _buttonList[COMMAND_KEY.Move].AddBinding("<Keyboard>/S");
    //    _buttonList[COMMAND_KEY.Move].AddBinding("<Keyboard>/D");

    //    _buttonList.Add(COMMAND_KEY.Jump, new InputAction(COMMAND_KEY.Jump.ToString(), binding: "<Keyboard>/space"));
    //    _buttonList.Add(COMMAND_KEY.LeftBtn, new InputAction(COMMAND_KEY.LeftBtn.ToString(), binding: "<Mouse>/leftButton"));
    //    _buttonList.Add(COMMAND_KEY.RightBtn, new InputAction(COMMAND_KEY.RightBtn.ToString(), binding: "<Mouse>/rightButton"));
    //    _buttonList.Add(COMMAND_KEY.HeadButt, new InputAction(COMMAND_KEY.HeadButt.ToString(), binding: "<Mouse>/middleButton"));
    //    _buttonList.Add(COMMAND_KEY.Skill, new InputAction(COMMAND_KEY.Skill.ToString(), binding: "<Keyboard>/R"));
    //    foreach (var button in _buttonList.Values) { button.Enable(); }
    //}
    //private InputManagerEntry AddButtonEntry(string buttonName, string inputString)
    //{
    //    InputManagerEntry entry = new InputManagerEntry
    //    {
    //        kind = InputManagerEntry.Kind.KeyOrButton,
    //        name = buttonName,
    //        btnPositive = inputString,
    //        btnNegative = ""
    //    };
    //    return entry;
    //}




    //public bool CheckInput(COMMAND_KEY commandKey, GetKeyType keyType)
    //{
    //    // COMMAND_KEY에 대응하는 KeyCode 배열 가져오기
    //    if (keyMap.TryGetValue(commandKey, out _keyCodes))
    //    {
    //        // 각 KeyCode에 대해 입력 확인
    //        foreach (KeyCode keyCode in _keyCodes)
    //        {
    //            _isEnabledKey = false;
    //            switch (keyType)
    //            {
    //                case GetKeyType.Press:
    //                    _isEnabledKey = Input.GetKey(keyCode);
    //                    break;
    //                case GetKeyType.Down:
    //                    _isEnabledKey = Input.GetKeyDown(keyCode);
    //                    break;
    //                case GetKeyType.Up:
    //                    _isEnabledKey = Input.GetKeyUp(keyCode);
    //                    break;
    //                default:
    //                    _isEnabledKey = false;
    //                    break;
    //            }

    //            // 입력이 발생하면 해당 커맨드를 실행하고 true 반환
    //            if (_isEnabledKey)
    //            {
    //                //ReserveCommand(commandKey);
    //                return true;
    //            }
    //        }
    //    }

    //    // 입력이 발생하지 않았거나 COMMAND_KEY에 대응하는 KeyCode 배열이 없는 경우 false 반환
    //    return false;
    //}






    //void OnKeyboardEvent(Define.KeyboardEvent evt)
    //{
    //    if (!photonView.IsMine || _actor.actorState == ActorState.Dead)
    //        return;

    //    if ((_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
    //        (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
    //        (_actor.debuffState & DebuffState.Stun) == DebuffState.Stun)
    //        return;

    //    _actor.PlayerController.OnKeyboardEvent_Move(evt);

    //    if (_actor.GrabState != Define.GrabState.EquipItem)
    //    {
    //        if(!((_actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted))
    //            _actor.PlayerController.OnKeyboardEvent_Skill(evt);
    //    }

    //    if (Input.GetKeyUp(KeyCode.F4))
    //    {
    //        KillOneself();
    //    }
    //}

    //#region 임시 자살 테스트용
    //void KillOneself()
    //{
    //    photonView.RPC("TestKill", RpcTarget.MasterClient, photonView.ViewID);
    //}

    //[PunRPC]
    //void TestKill(int ID)
    //{
    //    PhotonView pv = PhotonView.Find(ID);
    //    Actor ac = pv.transform.GetComponent<Actor>();
    //    StartCoroutine(ac.StatusHandler.ResetBodySpring());
    //    ac.actorState = Actor.ActorState.Dead;
    //    ac.StatusHandler._isDead = true;
    //    _actor.Health = 0;
    //    ac.InvokeDeathEvent();
    //    ac.InvokeStatusChangeEvent();
    //}

    //#endregion

    //void OnMouseEvent(Define.MouseEvent evt)
    //{
    //    if (!photonView.IsMine || _actor.actorState == ActorState.Dead)
    //        return;

    //    if ((_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
    //        (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
    //        (_actor.debuffState & DebuffState.Stun) == DebuffState.Stun)
    //        return;

    //    if (_actor.GrabState != Define.GrabState.EquipItem)
    //    {
    //        _actor.PlayerController.OnMouseEvent_Skill(evt);

    //        if (!((_actor.debuffState & DebuffState.Burn) == DebuffState.Burn))
    //            if (_actor.GrabState == Define.GrabState.PlayerLift)
    //            {
    //                _actor.Grab.OnMouseEvent_LiftPlayer(evt);
    //                return;
    //            }
    //            else
    //                _actor.PlayerController.OnMouseEvent_Grab(evt);

    //    }
    //    else
    //    {
    //        _actor.Grab.OnMouseEvent_EquipItem(evt);
    //    }
    //}
}
