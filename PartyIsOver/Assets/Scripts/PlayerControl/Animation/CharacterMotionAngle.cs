using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotionAngle : MonoBehaviour
{
    public enum AniAngle
    {
        Zero,
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left,
    }
    public Rigidbody[] StandardRigidbodies;
    public Transform[] ActionDirections;
    public Transform[] TargetDirections;
    public AniAngle[] ActionRotationDirections;
    public AniAngle[] TargetRotationDirections;
    public float[] AngleStabilities;
    public float[] AnglePowerValues;
}
