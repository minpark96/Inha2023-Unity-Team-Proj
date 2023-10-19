using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Define
{
    public enum WorldObject
    {
        Unknown,
        Player,
        Item,
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

    public enum Scene
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

    public enum CameraMode
    {

    }
}

