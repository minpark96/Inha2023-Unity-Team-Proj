using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionContext
{
    public int Id { get; set; }
    public bool IsMine { get; set; }
    public Vector3 Position {  get; set; }
    public int Layer { get; set; }

    public float InputDirX { get; set; }
    public float InputDirY { get; set; }
    public float InputDirZ { get; set; }

    public bool IsRunState { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsUpperActionProgress { get; set; }
    public bool IsLowerActionProgress { get; set; }
    public bool IsFlambe { get; set; }
    public bool IsMeowPunch { get; set; }
    public bool IsItemGrabbing { get; set; }


    public int[] LimbPositions { get; set; }
    public Define.Side PunchSide { get; set; }
    public Define.Side ItemHandleSide { get; set; }

    public InteractableObject LeftSearchTarget { get; set; }
    public InteractableObject RightSearchTarget { get; set; }
    public InteractableObject RightGrabObject { get; set; }
    public InteractableObject LeftGrabObject { get; set; }
    public InteractableObject EquipItem { get; set; }

    public FixedJoint RightGrabJoint { get; set; }
    public FixedJoint LeftGrabJoint { get; set; }

    public Vector3 LeftTargetDir { get; set; }
    public Vector3 RightTargetDir { get; set; }

    public void SetupAction(int id, bool isMine)
    {
        InputDirX = 0;
        InputDirY = 0;
        InputDirZ = 0f;
        IsRunState = false;
        IsGrounded = false;
        IsUpperActionProgress = false;
        IsLowerActionProgress = false;
        LimbPositions = new int[4];
        PunchSide = Define.Side.Left;
        IsMeowPunch = false;
        IsMine = isMine;
        Id = id;
    }
}
