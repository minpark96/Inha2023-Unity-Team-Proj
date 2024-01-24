using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AniFrameData;
using static CharacterPhysicsMotion;

public class JumpingAnimation : MonoBehaviour
{
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
    public void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
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
    void AniForceVelocityChange(CharacterPhysicsMotion[] _forceSpeed, int _elementCount, Vector3 _dir)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ForceDirections[i] == ForceDirection.Zero || _forceSpeed[_elementCount].ForceDirections[i] == ForceDirection.ZeroReverse)
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ActionPowerValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ActionPowerValues[i], ForceMode.Impulse);
            }
        }
    }

    Vector3 GetForceDirection(CharacterPhysicsMotion data, int index)
    {
        ForceDirection _rollState = data.ForceDirections[index];
        Vector3 _direction;

        switch (_rollState)
        {
            case ForceDirection.Zero:
                _direction = new Vector3(0, 0, 0);
                break;
            case ForceDirection.ZeroReverse:
                _direction = new Vector3(-1, -1, -1);
                break;
            case ForceDirection.Forward:
                _direction = -data.StandardRigidbodies[index].transform.up;
                break;
            case ForceDirection.Backward:
                _direction = data.StandardRigidbodies[index].transform.up;
                break;
            case ForceDirection.Up:
                _direction = data.StandardRigidbodies[index].transform.forward;
                break;
            case ForceDirection.Down:
                _direction = -data.StandardRigidbodies[index].transform.forward;
                break;
            case ForceDirection.Left:
                _direction = -data.StandardRigidbodies[index].transform.right;
                break;
            case ForceDirection.Right:
                _direction = data.StandardRigidbodies[index].transform.right;
                break;
            default:
                _direction = Vector3.zero;
                break;
        }
        return _direction;
    }

}
