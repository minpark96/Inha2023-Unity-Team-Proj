using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AniAngleData;
using static AniFrameData;

public class AnimationData
{
    public AniFrameData[] ForceAniData;
    public AniAngleData[] AngleAniData;

    public Vector3 GetForceDirection(AniFrameData data, int index)
    {
        ForceDirection _rollState = data.ForceDirections[index];
        Vector3 _direction;

        switch (_rollState)
        {
            case ForceDirection.Zero:
                _direction = new Vector3(0, 0, 0);
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

    public void AniForce(AniFrameData[] _forceSpeed, int _elementCount, Vector3 _dir = default, float _punchpower = 1f)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ForceDirections[i] == ForceDirection.Zero)
            {
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ForcePowerValues[i] * _punchpower, ForceMode.Impulse);
            }
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i] * _punchpower, ForceMode.Impulse);
            }
        }
    }

    public Vector3 GetAngleDirection(AniAngle _angleState, Transform _Transformdirection)
    {
        Vector3 _direction;

        switch (_angleState)
        {
            case AniAngle.Zero:
                _direction = Vector3.zero;
                break;
            case AniAngle.Forward:
                _direction = -_Transformdirection.transform.up;
                break;
            case AniAngle.Backward:
                _direction = _Transformdirection.transform.up;
                break;
            case AniAngle.Up:
                _direction = _Transformdirection.transform.forward;
                break;
            case AniAngle.Down:
                _direction = -_Transformdirection.transform.forward;
                break;
            case AniAngle.Left:
                _direction = -_Transformdirection.transform.right;
                break;
            case AniAngle.Right:
                _direction = _Transformdirection.transform.right;
                break;
            default:
                _direction = Vector3.zero;
                break;
        }

        return _direction;
    }

    public void AniAngleForce(AniAngleData[] _aniAngleData, int _elementCount, Vector3 _vector = default)//default´Â vector3.zero
    {
        for (int i = 0; i < _aniAngleData[_elementCount].ActionRigidbodies.Length; i++)
        {
            Vector3 _angleDirection = GetAngleDirection(_aniAngleData[_elementCount].StandardDirections[i],
                _aniAngleData[_elementCount].StandardPart[i]);
            Vector3 _targetDirection = GetAngleDirection(_aniAngleData[_elementCount].TargetDirections[i],
                _aniAngleData[_elementCount].TargetPart[i]);

            AlignToVector(_aniAngleData[_elementCount].ActionRigidbodies[i], _angleDirection, _vector + _targetDirection,
                _aniAngleData[_elementCount].AngleStability[i], _aniAngleData[_elementCount].AnglePowerValues[i]);
        }
    }

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
}
