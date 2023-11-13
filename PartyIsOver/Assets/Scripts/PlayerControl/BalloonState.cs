using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BalloonState : MonoBehaviour
{
    public PlayerController PlayerController;
    public Transform CameraArm;
    private Actor _actor;
    private List<Quaternion> _initialRotations = new List<Quaternion>();

    public float BalloonDuration;
    public float RotateAngle;
    public bool  IsGrounded;
    private Vector3 _moveDir;

    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
        _actor = GetComponentInParent<Actor>();
        CameraArm = transform.GetChild(0).GetChild(0);
}


    public IEnumerator BalloonShapeOn()
    {
        // 전 hip 각도 저장
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _initialRotations.Add(_actor.BodyHandler.BodyParts[i].PartTransform.localRotation);
        }

        float startTime = Time.time;
        float duration = 2f;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float scaleValue1 = Mathf.Lerp(1f, 2f, t);
            float scaleValue2 = Mathf.Lerp(1f, 2.5f, t);
            float scaleValue3 = Mathf.Lerp(1f, 3.3f, t);

            _actor.BodyHandler.Head.transform.localScale = new Vector3(scaleValue1, scaleValue1, 0.8f);
            _actor.BodyHandler.Chest.transform.localScale = new Vector3(scaleValue2, scaleValue2, scaleValue3);
            _actor.BodyHandler.Waist.transform.localScale = new Vector3(scaleValue2, scaleValue2, scaleValue3);

            yield return null;
        }

        for (int i = 4; i < 13; i++)
        {
            if (i >= 7 && i <= 9) continue;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = true;
        }
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.velocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.useGravity = false;
        }

        _actor.BodyHandler.BodyParts[2].AddComponent<BalloonCollision>();
        _actor.BodyHandler.BodyParts[2].GetComponent<Collider>().isTrigger = true;

        yield return new WaitForSeconds(BalloonDuration);

        BalloonShapeOff();
    }

    public void BalloonShapeOff()
    {
        _actor.debuffState = Actor.DebuffState.Default;

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartTransform.localRotation = _initialRotations[i];
        }

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.velocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.useGravity = true;
        }


        _actor.BodyHandler.BodyParts[0].transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.BodyParts[1].transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.BodyParts[2].transform.localScale = new Vector3(1, 1, 1);


        for (int i = 4; i < 13; i++)
        {
            if (i >= 7 && i <= 9) continue;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = false;
        }

        Destroy(_actor.BodyHandler.BodyParts[2].GetComponent<BalloonCollision>());
        _actor.BodyHandler.BodyParts[2].GetComponent<Collider>().isTrigger = false;
        _actor.PlayerController.isBalloon = false;
    }

    public void BalloonMove()
    {
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
        }

        Vector3 lookForward = new Vector3(CameraArm.forward.x, 0f, CameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(CameraArm.right.x, 0f, CameraArm.right.z).normalized;
        _moveDir = lookForward * PlayerController.MoveInput.z + lookRight * PlayerController.MoveInput.x;
        Vector3 rotateDir = new Vector3(_moveDir.x, 0, 0);

        _actor.BodyHandler.Hip.PartRigidbody.AddForce(lookForward * 350f * Time.deltaTime);
        _actor.BodyHandler.Hip.PartTransform.Rotate(rotateDir, RotateAngle);
    }

    public void BalloonJump()
    {
        IsGrounded = false;
        for (int i = 0; i < PlayerController.MoveForceJumpAniData.Length; i++)
        {
            PlayerController.AniForce(PlayerController.MoveForceJumpAniData, i, Vector3.up);
            if (i == 2)
                PlayerController.AniForce(PlayerController.MoveForceJumpAniData, i, Vector3.down);
        }
        for (int i = 0; i < PlayerController.MoveAngleJumpAniData.Length; i++)
        {
            PlayerController.AniAngleForce(PlayerController.MoveAngleJumpAniData, i, _moveDir + new Vector3(0, 0.2f, 0f));
        }
    }
}
