using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CmdInAirMove : PlayerCommand
{
    public CmdInAirMove(Actor actor)
    {
        bodyHandler = actor.BodyHandler;
    }

    public override void Execute(Vector3 moveDir = default)
    {
        bodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + moveDir), ForceMode.VelocityChange);
        bodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(bodyHandler.Chest.PartRigidbody, -bodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(bodyHandler.Chest.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, -bodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, -bodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

        //Fall상태로 빼야 할수도
        bodyHandler.Hip.PartRigidbody.AddForce(moveDir.normalized * RunSpeed * runSpeedOffset * Time.deltaTime * 0.5f);

        if (bodyHandler.Hip.PartRigidbody.velocity.magnitude > MaxSpeed)
            bodyHandler.Hip.PartRigidbody.velocity = bodyHandler.Hip.PartRigidbody.velocity.normalized * MaxSpeed;
    }
}
