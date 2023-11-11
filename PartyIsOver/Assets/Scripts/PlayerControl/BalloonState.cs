using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BalloonState : MonoBehaviour
{
    public Transform CameraArm;
    private Actor _actor;
    private List<Quaternion> _initialRotations = new List<Quaternion>();


    void Start()
    {
        _actor = transform.root.GetComponent<Actor>();
        CameraArm = transform.GetChild(0).GetChild(0);

    }


    public IEnumerator BalloonShapeOn()
    {
        // 전 hip 각도 저장
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _initialRotations.Add(_actor.BodyHandler.BodyParts[i].PartTransform.localRotation);
        }

        _actor.BodyHandler.Head.transform.localScale = new Vector3(2f, 2f, 0.8f);
        _actor.BodyHandler.Chest.transform.localScale = new Vector3(2.5f, 2.5f, 3.3f);
        _actor.BodyHandler.Waist.transform.localScale = new Vector3(2.5f, 2.5f, 3.3f);

        //float startTime = Time.time;
        //while(Time.time + startTime < 2f)
        //{

        //}

        //456 789 101112 131415
        for (int i = 4; i < 7; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = true;
        }
        for (int i = 10; i < 13; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = true;
        }

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.velocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.useGravity = false;
        }

        _actor.BodyHandler.BodyParts[2].AddComponent<LimbCollision>();

        yield return new WaitForSeconds(5.0f);

        BalloonShapeOff();
    }

    public void BalloonShapeOff()
    {
        _actor.debuffState = Actor.DebuffState.Default;

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.velocity = Vector3.zero;

            _actor.BodyHandler.BodyParts[i].PartTransform.localRotation = _initialRotations[i];
        }


        _actor.BodyHandler.BodyParts[0].transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.BodyParts[1].transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.BodyParts[2].transform.localScale = new Vector3(1, 1, 1);


        for (int i = 4; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = false;
        }
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count - 1; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.useGravity = true;
        }
      

        Destroy(_actor.BodyHandler.BodyParts[2].GetComponent<LimbCollision>());
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
        Vector3 _moveDir = lookForward * _actor.PlayerController.MoveInput.z + lookRight * _actor.PlayerController.MoveInput.x;

        _actor.BodyHandler.Hip.PartRigidbody.AddForce(_moveDir.normalized * 100f * 350f * Time.deltaTime);

        _actor.BodyHandler.Hip.PartTransform.Rotate(lookForward, 50f);


    }

    //public void BalloonJump()
    //{
    //    float startTime = Time.time;
    //    float balloonDuration = 2f;

    //    while (startTime - Time.time < balloonDuration)
    //    {

    //    }
    //    //코루틴
    //    Jump();
    //}
}
