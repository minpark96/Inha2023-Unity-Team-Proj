using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Define
{
    public const byte MAX_PLAYERS_PER_ROOM = 6;

    public enum BodyPart
    {
        FootL,
        FootR,
        LegLowerL,
        LegLowerR,
        LegUpperL,
        LegUpperR,
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
    public enum UIEvent
    {
        Click,
        Drag,
    }
    public enum AIState
    {
        Idle,
        Move,
        Find,
        Attack,
    }


    public enum SceneType
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
        InGameStackSound,
        MagneticWarning,
        SnowStormWarning,
        Maxcount,
    }

    public enum ObjectType
    {
        None,
        Player,
        Item,
        Object,
        Wall,
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
        Consumable,
    }

    public enum SpawnableItemType
    {
        TwoHanded,
        Ranged,
        Consumable,



        End,
    }

    public enum TwoHandedItemType
    {
        FrozenMeat,

        End,
    }

    public enum RangedItemType
    {
        IceGun,
        TaserGun,

        End,
    }

    public enum ConsumableItemType
    {
        IceCream,
        SpicyOnion,
        Mushroom,
        Donut,

        End,
    }

    public enum Layer
    {
        Ground = 9,
        Item = 10,
        ClimbObject = 11,
        InteractableObject = 12,
        TriggerObject = 13,
        IceFloor = 14,

        Player1 = 26,
        Player2 = 27,
        Player3 = 28,
        Player4 = 29,
        Player5 = 30,
        Player6 = 31,
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


    public enum Area
    {
        Floor,
        Inside,
        Outside,
        Default,
    }
    public enum PlayerState
    {
        IndexLowerStart,

        LowerIdle,
        Moving,
        Jumping,
        DropKick,

        IndexLowerEnd,
        IndexUpperStart,

        UpperIdle,
        PunchAndGrabReady,
        Punch,
        Grabbing,
        SkillReady,
        Skill,
        HeadButt,
        EquipItem,
        LiftObject,
        Climb,

        IndexUpperEnd,
    }

    public enum GetKeyType
    {
        Press,
        Down,
        Up,
        None,
    }

    public enum COMMAND_KEY
    {
        Move           =    0x1,
        LeftBtn        =    0x2,
        RightBtn       =    0x4,
        Jump           =    0x8,
        Skill          =    0x10,
        Charge         =    0x20, 
        ResetCharge    =    0x40, 
        HeadButt       =    0x80, 
        TargetSearch   =    0x100,
        Grabbing       =    0x200,
        FixJoint       =    0x400,
        DestroyJoint   =    0x800,

        None = 0x8000,
    }

    public enum AniFrameData
    {
        DropAniData,
        RightPunchingAniData,
        LeftPunchingAniData,
        JumpAniForceData,
        HeadingAniData,
        RSkillAniData,
        ItemTwoHandAniData,
        PotionReadyAniData,
        PotionDrinkingAniData,
        ItemTwoHandLeftAniData,
        PotionThrowAniData,

        End,
    }

    public enum AniAngleData
    {
        RightPunchAniData,
        LeftPunchAniData,
        RightPunchResettingAniData,
        LeftPunchResettingAniData,
        MoveAngleJumpAniData,
        HeadingAngleAniData,
        RSkillAngleAniData,
        ItemTwoHandAngleData,
        PotionAngleAniData,
        ItemTwoHandLeftAngleData,
        PotionThrowAngleData,

        End,
    }

    public enum AnimDirection
    {
        Zero,
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left,
    }

    public enum AnimForceValue
    {
        ActionCount,
        PartCount,
        StandardPart,
        ActionPart,
        ForceDirection,
        ForcePowerValues,
    }

    public enum AnimRotateValue
    {
        ActionCount,
        PartCount,
        ActionRigidbodies,
        StandardPart,
        TartgetPart,
        StandardDirections,
        TargetDirections,
        AngleStability,
        AnglePowerValues,
    }

    public enum BodyPose
    {
        Bent = 0,
        Forward = 1,
        Straight = 2,
        Behind = 3,

        End,
    }

    public enum Side
    {
        Left = 0,
        Right = 1,
        Both = 2,
    }

 

    public enum limbPositions
    {
        leftArmPose,
        rightArmPose,
        leftLegPose,
        rightLegPose,
        End,
    }


    public enum PlayerDatas
    {
        dirX,
        dirY,
        dirZ,
        RunState,

        End,
    }
}


