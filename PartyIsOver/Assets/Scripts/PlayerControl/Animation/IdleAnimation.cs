using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimation : Idle
{
    public IdleAnimation(MovementSM stateMachine) : base(stateMachine) { }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        AlignToVector(sm.PlayerCharacter.bodyHandler.Head.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Head.transform.up, moveDir + new Vector3(0f, 0.2f, 0f), 0.1f, 2.5f * 1);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Head.PartRigidbody, sm.PlayerCharacter.bodyHandler.Head.transform.forward, Vector3.up, 0.1f, 2.5f * 1);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Chest.transform.up, moveDir, 0.1f, 4f * 1);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 4f * 1);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Waist.transform.up, moveDir, 0.1f, 4f * 1);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, sm.PlayerCharacter.bodyHandler.Waist.transform.forward, Vector3.up, 0.1f, 4f * 1);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, sm.PlayerCharacter.bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 3f * 1);
    }
}
