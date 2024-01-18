using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommand : ICommand
{
    protected BodyHandler bodyHandler;
    protected AnimationPlayer animPlayer;
    protected AnimationData animData;


    protected float RunSpeed;
    protected float MaxSpeed;
    protected float runSpeedOffset = 350f;


    protected Vector3 runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    protected Vector3 runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    protected Vector3 runVectorForce10 = new Vector3(0f, 0f, 0.8f);

    protected float applyedForce = 800f;

    public virtual void Execute(Vector3 moveDir = default)
    {
       
    }

    protected void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
    {
        if (part == null)
        {
            return;
        }
        Vector3 vector = Vector3.Cross(Quaternion.AngleAxis(part.angularVelocity.magnitude * 57.29578f * stability / speed, part.angularVelocity) * alignmentVector, targetVector * 10f);
        if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z))
        {
            part.AddTorque(vector * speed * speed);
            {
                Debug.DrawRay(part.position, alignmentVector * 0.2f, Color.red, 0f, depthTest: false);
                Debug.DrawRay(part.position, targetVector * 0.2f, Color.green, 0f, depthTest: false);
            }
        }
    }
}
