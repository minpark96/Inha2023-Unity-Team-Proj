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
    public Rigidbody[] ReferenceRigidbodies;
    public Rigidbody[] ActionRigidbodies;
    public ForceDirection[] ActionForceDirections;
    public float[] ActionForceValues;



}
