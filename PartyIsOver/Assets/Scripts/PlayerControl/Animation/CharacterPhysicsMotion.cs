using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhysicsMotion : MonoBehaviour
{
    public enum ForceDirection
    {
        Zero,
        ZeroReverse,
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left,
    }
    public Rigidbody[] StandardRigidbodies;
    public Rigidbody[] ActionRigidbodies;
    public ForceDirection[] ForceDirections;
    public float[] ActionPowerValues;
}
