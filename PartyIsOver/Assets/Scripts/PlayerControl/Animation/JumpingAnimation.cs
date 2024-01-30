using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AniFrameData;
using static CharacterPhysicsMotion;

public class JumpingAnimation : Jumping
{
    public JumpingAnimation(MovementSM stateMachine) : base(stateMachine) { }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, -sm.BodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, -sm.BodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, -sm.BodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, sm.BodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

    }
    //점프 하는 애니메이션
    /*
    if (isStateChange)
    {
            isGrounded = false;
            for (int i = 0; i<MoveForceJumpAniData.Length; i++)
            {
                AniForceVelocityChange(MoveForceJumpAniData, i, Vector3.up);
                if (i == 2)
                    AniForce(MoveForceJumpAniData, i, Vector3.down);
     }
for (int i = 0; i < MoveAngleJumpAniData.Length; i++)
{
    AniAngleForce(MoveAngleJumpAniData, i, _moveDir + new Vector3(0, 0.2f, 0f));
}

        }

        _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
    */

    void AniForceVelocityChange(CharacterPhysicsMotion[] _forceSpeed, int _elementCount, Vector3 _dir)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].ReferenceRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ActionForceDirections[i] == ForceDirection.Zero || _forceSpeed[_elementCount].ActionForceDirections[i] == ForceDirection.ZeroReverse)
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ActionForceValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ActionForceValues[i], ForceMode.Impulse);
            }
        }
    }

}
