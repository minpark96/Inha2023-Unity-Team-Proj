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
    public Transform[] ActionDirection;
    public Transform[] TargetDirection;
    public AniAngle[] ActionAngleDirections;
    public AniAngle[] TargetAngleDirections;
    public float[] AngleStability;
    public float[] AnglePowerValues;
}
