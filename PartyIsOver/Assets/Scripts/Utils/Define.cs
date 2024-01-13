using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Define
{
    public const byte MAX_PLAYERS_PER_ROOM = 6;

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
        Ground=9,
        Item = 10,
        ClimbObject = 11,
        InteractableObject =12,
        TriggerObject = 13,
        IceFloor = 14,

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

    public enum AIState
    {
        Idle,
        Move,
        Find,
        Attack,
    }

    public enum Area
    {
        Floor,
        Inside,
        Outside,
        Default,
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
        W,
        A,
        S,
        D,
        LeftBtn,
        RightBtn,
        Wheel,
        Jump,

    }

    public enum AniFrameData
    {
        DropAniData,
        RightPunchingAniData,
        LeftPunchingAniData,
        MoveForceJumpAniData,
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

}


