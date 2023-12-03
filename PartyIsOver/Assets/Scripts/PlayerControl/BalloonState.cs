using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BalloonState : MonoBehaviour
{
    public float BalloonDuration;
    //public bool IsGrounded;
    //public float RotateAngle = 8f;
    //public float Force = 10000f;

    private PlayerController _playerController;
    private Transform _cameraArm;
    private Actor _actor;

    private Vector3 _moveDir;
    //private float _originalMass;
    //private float _totalAngle = 0f;


    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _actor = GetComponent<Actor>();
        _cameraArm = transform.GetChild(0).GetChild(0);
        //_originalMass = _actor.BodyHandler.Hip.PartRigidbody.mass;
    }

    public IEnumerator BalloonShapeOn()
    {
        _playerController.isBalloon = true;

        for (int i = (int)Define.BodyPart.Hip; i <= (int)Define.BodyPart.Head; i++)
        {
            RigidbodyConstraints constraints = _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints;
            constraints |= RigidbodyConstraints.FreezeRotationY;
            constraints |= RigidbodyConstraints.FreezeRotationZ;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints = constraints;
        }
        for (int i = (int)Define.BodyPart.LeftArm; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = true;
        }

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.velocity = Vector3.zero;
        }

        _actor.BodyHandler.Waist.AddComponent<LimbCollision>();
        _actor.BodyHandler.Waist.GetComponent<CapsuleCollider>().radius = 0.3f;
        _actor.BodyHandler.Waist.GetComponent<Collider>().isTrigger = true;

        float startTime = Time.time;
        float duration = 1f;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float scaleValue1 = Mathf.Lerp(1f, 2f, t);
            float scaleValue2 = Mathf.Lerp(1f, 3f, t);
            float scaleValue3 = Mathf.Lerp(1f, 4f, t);
            float scaleValue4 = Mathf.Lerp(1f, 5f, t);
            float scaleValue5 = Mathf.Lerp(1f, 7f, t);

            _actor.BodyHandler.Head.transform.localScale = new Vector3(scaleValue2, scaleValue1, scaleValue2);
            _actor.BodyHandler.Chest.transform.localScale = new Vector3(scaleValue3, scaleValue4, scaleValue5);
            _actor.BodyHandler.Waist.transform.localScale = new Vector3(scaleValue2, scaleValue1, scaleValue4);

            yield return null;
        }

        yield return new WaitForSeconds(BalloonDuration);

        BalloonShapeOff();
    }

    public void BalloonShapeOff()
    {
        _actor.debuffState = Actor.DebuffState.Default;

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartTransform.localRotation = _playerController.RotationsForBalloon[i];
        }

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.velocity = Vector3.zero;
        }
        _actor.BodyHandler.Head.transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.Chest.transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.Waist.transform.localScale = new Vector3(1, 1, 1);

        for (int i = (int)Define.BodyPart.Hip; i <= (int)Define.BodyPart.Head; i++)
        {
            RigidbodyConstraints constraints = _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints;

            if(i != (int)Define.BodyPart.Hip)
                constraints &= ~RigidbodyConstraints.FreezeRotationY;

            constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints = constraints;
        }
        for (int i = (int)Define.BodyPart.LeftArm; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = false;
        }

        Destroy(_actor.BodyHandler.Waist.GetComponent<LimbCollision>());
        _actor.BodyHandler.Waist.GetComponent<CapsuleCollider>().radius = 0.25f;
        _actor.BodyHandler.Waist.GetComponent<Collider>().isTrigger = false;
        _actor.PlayerController.isBalloon = false;
    }


    public void BalloonMove()
    {
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
        }

        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        _moveDir = lookForward * _playerController.MoveInput.z + lookRight * _playerController.MoveInput.x;


        if (_playerController.MoveInput.z == 1)
        {
            _actor.BodyHandler.Hip.PartRigidbody.AddForce(_moveDir * 350f * Time.deltaTime);
            Vector3 rotateDirX = new Vector3(_moveDir.x, 0, 0);
            _actor.BodyHandler.Hip.PartTransform.Rotate(rotateDirX, 6);
        }

        //if (PlayerController.MoveInput.x == 1 || PlayerController.MoveInput.x == -1)
        //{
        //    Actor.BodyHandler.Hip.PartRigidbody.mass = 18;

        //    Vector3 rotateDirZ = new Vector3(0, 0, _moveDir.z);

        //    if (_totalAngle <= 180f)
        //    {
        //        for (int i = (int)Define.BodyPart.Hip; i < (int)Define.BodyPart.Head; i++)
        //        {
        //            Actor.BodyHandler.BodyParts[i].PartTransform.Rotate(rotateDirZ, RotateAngle * 0.5f);
        //        }

        //        _totalAngle += RotateAngle;
        //    }
        //}
        //else
        //{
        //    _totalAngle = 0;
        //    Actor.BodyHandler.Hip.PartRigidbody.mass = _originalMass;
        //}
    }

    //public IEnumerator BalloonSpin()
    //{
    //    Vector3 lookForward = new Vector3(CameraArm.forward.x, 0f, CameraArm.forward.z).normalized;
    //    Vector3 rotateDirZ = new Vector3(0, 0, 1);
    //    float startTime = Time.time;
    //    float totalAngle = 0f;

    //    while (Time.time - startTime < 2f)
    //    {
    //        for (int i = 0; i < Actor.BodyHandler.BodyParts.Count; i++)
    //        {
    //            Actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
    //        }

    //        if (totalAngle <= 180)
    //        {
    //            for (int i = 0; i < 4; i++)
    //            {
    //                Actor.BodyHandler.BodyParts[i].PartTransform.Rotate(rotateDirZ, RotateAngle);
    //            }

    //            totalAngle += RotateAngle;
    //        }
    //        else
    //            break;

    //        yield return new WaitForSeconds(0.02f);
    //    }

    //    // Force : 3¸¸ÀÏ¶§ ÀÌ»Ý
    //    Actor.BodyHandler.Hip.PartRigidbody.AddForce(lookForward * 1000f * Force * Time.deltaTime);

    //}
}
