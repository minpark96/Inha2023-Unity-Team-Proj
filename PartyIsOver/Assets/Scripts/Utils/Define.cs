using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Define
{
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


    public enum ItemType
    {
        OneHanded,
        TwoHanded,
        Ranged,
        Potion,
    }

    public enum State
    {
        Idle,
        Move,
        Roll,
        Punch,
        Kick,
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


