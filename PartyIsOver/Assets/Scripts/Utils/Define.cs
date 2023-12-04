using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Define
{
    public enum BodyPart
    {
        LeftFoot,
        RightFoot,
        LeftLeg,
        RightLeg,
        LeftThigh,
        RightThigh,
        Hip,
        Waist,
        Chest,
        Head,
        LeftArm,
        RightArm,
        LeftForeArm,
        RightForeArm,
        LeftHand,
        RightHand,
        Ball,
    }

    public enum Effect
    {
        UIEffect,
        PlayerEffect,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Start,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        UISound,
        UIInGameSound,
        PlayerEffect,
        Maxcount,
    }

    public enum WorldObject
    {
        Unknown,
        Player,
        Item,
    }

    public enum GrabObjectType
    {
        None,
        Player,
        Item,
        Object,
    }

    public enum GrabState
    {
        None,
        EquipItem,
        PlayerLift,
        Climb,
    }

    public enum RangeWeapon
    {
        IceGun,
        StunGun
    }

    public enum ItemType
    {
        None,
        OneHanded,
        TwoHanded,
        Gravestone,
        Ranged,
        Potion,
    }



    public enum Layer
    {
        Item = 10,
        ClimbObject = 11,
        InteractableObject =12,

        Player1 = 26,
        Player2 = 27,
        Player3 = 28,
        Player4 = 29,
        Player5 = 30,
        Player6 = 31,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    public enum KeyboardEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
        Hold,
        Charge,
    }


    public enum CameraMode
    {

    }

    public enum AIState
    {
        Idle,
        Move,
        Find,
        Attack,
    }
}


