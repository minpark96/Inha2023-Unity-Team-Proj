using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext
{
    public float DirX { get; set; }
    public float DirY { get; set; }
    public float DirZ { get; set; }
    public bool IsRunState { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsUpperActionProgress { get; set; }
    public bool IsLowerActionProgress { get; set; }
    public bool IsEquipItem { get; set; }
    public bool IsMeowPunch { get; set; }
    public bool IsLeftGrab { get; set; }
    public bool IsRightGrab { get; set; }

    public int[] LimbPositions { get; set; }
    public Define.Side Side { get; set; }

    public InteractableObject LeftSearchTarget { get; set; }
    public InteractableObject RightSearchTarget { get; set; }

    public Vector3 LeftSearchTargeDir { get; set; }
    public Vector3 RightSearchTargeDir { get; set; }
}
