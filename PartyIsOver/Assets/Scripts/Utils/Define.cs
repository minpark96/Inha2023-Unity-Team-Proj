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
        Effect,
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


