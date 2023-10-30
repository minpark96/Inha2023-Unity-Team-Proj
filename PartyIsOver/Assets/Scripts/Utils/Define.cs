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
    public enum WorldObject
    {
        Unknown,
        Player,
        Item,
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

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
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
}


