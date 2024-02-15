using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext
{
    public int Id { get; set; }
    public bool IsMine { get; set; }
    public Vector3 Position {  get; set; }
    public int Layer { get; set; }

    public float DirX { get; set; }
    public float DirY { get; set; }
    public float DirZ { get; set; }

    public bool IsRunState { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsUpperActionProgress { get; set; }
    public bool IsLowerActionProgress { get; set; }
    public bool IsMeowPunch { get; set; }

    public int[] LimbPositions { get; set; }
    public Define.Side Side { get; set; }

    public InteractableObject LeftSearchTarget { get; set; }
    public InteractableObject RightSearchTarget { get; set; }
    public InteractableObject RightGrabObject { get; set; }
    public InteractableObject LeftGrabObject { get; set; }
    public InteractableObject EquipItem { get; set; }

    public FixedJoint RightGrabJoint { get; set; }
    public FixedJoint LeftGrabJoint { get; set; }

    public Vector3 LeftTargetDir { get; set; }
    public Vector3 RightTargetDir { get; set; }
}
