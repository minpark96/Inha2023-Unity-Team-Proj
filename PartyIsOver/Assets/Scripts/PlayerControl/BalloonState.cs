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
    public bool IsGrounded;
    private Vector3 _moveDir;
    Transform Sphere;

    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
        _actor = GetComponentInParent<Actor>();
        CameraArm = transform.GetChild(0).GetChild(0);
        Sphere = _actor.BodyHandler.Hip.transform.GetChild(4);
    }


    public IEnumerator BalloonShapeOn()
    {
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
            float scaleValue2 = Mathf.Lerp(1f, 3f, t);
            float scaleValue3 = Mathf.Lerp(1f, 4f, t);
            float scaleValue4 = Mathf.Lerp(1f, 5f, t);
            float scaleValue5 = Mathf.Lerp(1f, 7f, t);

            _actor.BodyHandler.Head.transform.localScale = new Vector3(scaleValue2, scaleValue1, scaleValue2);
            _actor.BodyHandler.Chest.transform.localScale = new Vector3(scaleValue3, scaleValue4, scaleValue5);
            _actor.BodyHandler.Waist.transform.localScale = new Vector3(scaleValue2, scaleValue1, scaleValue4);

            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            RigidbodyConstraints constraints = _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints;
            constraints |= RigidbodyConstraints.FreezeRotationY;
            constraints |= RigidbodyConstraints.FreezeRotationZ;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints = constraints;
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
        }

        _actor.BodyHandler.BodyParts[2].AddComponent<LimbCollision>();
        _actor.BodyHandler.BodyParts[2].GetComponent<CapsuleCollider>().radius = 0.3f;
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
        }

        _actor.BodyHandler.BodyParts[0].transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.BodyParts[1].transform.localScale = new Vector3(1, 1, 1);
        _actor.BodyHandler.BodyParts[2].transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < 3; i++)
        {
            RigidbodyConstraints constraints = _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints;
            constraints &= ~RigidbodyConstraints.FreezeRotationY;
            constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.constraints = constraints;
        }
        for (int i = 4; i < 13; i++)
        {
            if (i >= 7 && i <= 9) continue;
            _actor.BodyHandler.BodyParts[i].PartRigidbody.freezeRotation = false;
        }

        Destroy(_actor.BodyHandler.BodyParts[2].GetComponent<LimbCollision>());
        _actor.BodyHandler.BodyParts[2].GetComponent<CapsuleCollider>().radius = 0.25f;
        _actor.BodyHandler.BodyParts[2].GetComponent<Collider>().isTrigger = false;
        _actor.PlayerController.isBalloon = false;
    }

    public void BalloonMove()
    {
        PlayerController.BalloonJump = 0;
        PlayerController.BalloonDrop = false;

        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
        }

        Vector3 lookForward = new Vector3(CameraArm.forward.x, 0f, CameraArm.forward.z).normalized; // z
        Vector3 lookRight = new Vector3(CameraArm.right.x, 0f, CameraArm.right.z).normalized; // x
        _moveDir = lookForward * PlayerController.MoveInput.z + lookRight * PlayerController.MoveInput.x;


        //_actor.BodyHandler.Hip.PartRigidbody.AddForce(_moveDir * 350f * Time.deltaTime);

        // Rotate 사용
        //Vector3 rotateDirX = new Vector3(_moveDir.x, 0, 0);
        //_actor.BodyHandler.Hip.PartTransform.Rotate(rotateDirX, RotateAngle);
        //Vector3 rotateDirZ = new Vector3(0, 0, _moveDir.z);
        //_actor.BodyHandler.Hip.PartTransform.Rotate(rotateDirZ, RotateAngle);

        // Align 사용하기
        //Debug.Log("align");
        //AlignToVector(_actor.BodyHandler.Hip.PartRigidbody, _actor.BodyHandler.Hip.transform.forward, Vector3.up, 0.1f,100000f);

        // 구 움직이기

        //Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
        //Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);
        //_actor.BodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        //_actor.BodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        //AlignToVector(_actor.BodyHandler.Chest.PartRigidbody, -_actor.BodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * 800f);
        //AlignToVector(_actor.BodyHandler.Chest.PartRigidbody, _actor.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * 800f);
        //AlignToVector(_actor.BodyHandler.Waist.PartRigidbody, -_actor.BodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * 800f);
        //AlignToVector(_actor.BodyHandler.Waist.PartRigidbody, _actor.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * 800f);
        //AlignToVector(_actor.BodyHandler.Hip.PartRigidbody, -_actor.BodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * 800f);
        //AlignToVector(_actor.BodyHandler.Hip.PartRigidbody, _actor.BodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * 800f);

        PlayerController.Move();
        Sphere.Rotate(new Vector3(_moveDir.x, 0, 0), RotateAngle);






        
    }

    public void BalloonSpin()
    {
        Debug.Log("Spin");
        for (int i = 0; i < _actor.BodyHandler.BodyParts.Count; i++)
        {
            _actor.BodyHandler.BodyParts[i].PartRigidbody.angularVelocity = Vector3.zero;
        }

        //_actor.BodyHandler.Chest.PartRigidbody.AddForce(-Vector3.up * 3500f * Time.deltaTime);
        //_actor.BodyHandler.Chest.PartTransform.Rotate(-Vector3.up, 30 * Time.deltaTime);
        //_actor.BodyHandler.Waist.PartRigidbody.AddForce(-Vector3.up * 3500f * Time.deltaTime);
        //_actor.BodyHandler.Waist.PartTransform.Rotate(-Vector3.up, 30 * Time.deltaTime);

        //StartCoroutine(PlayerController.balloon(0));

        Vector3 lookForward = new Vector3(CameraArm.forward.x, 0f, CameraArm.forward.z).normalized; // z
        Vector3 lookRight = new Vector3(CameraArm.right.x, 0f, CameraArm.right.z).normalized; // x
        _moveDir = lookForward * PlayerController.MoveInput.z + lookRight * PlayerController.MoveInput.x;
        Vector3 rotateDirZ = new Vector3(0, 0, _moveDir.z);


        _actor.BodyHandler.Hip.PartRigidbody.AddForce(_moveDir * 350f * Time.deltaTime);
        _actor.BodyHandler.Hip.PartTransform.Rotate(rotateDirZ, RotateAngle);


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
