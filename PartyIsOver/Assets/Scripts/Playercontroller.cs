using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("앞뒤 속도")]
    public float Speed;
    [Header("점프 힘")]
    public float JumpForce;

    [SerializeField]
    private Rigidbody _hips;
    [SerializeField]
    private Transform _cameraArm;

    [SerializeField]
    private BodyHandeler bodyHandeler;
    

    public bool isGrounded;
    public bool isRun;
    public bool isMove;
    public bool isDuck;

    private Vector3 _moveInput;
    private Vector3 _moveDir;
    private float _rotationSpeed = 10f;



    private float _cycleTimer = 0;
    private float _cycleSpeed;
    private float _applyedForce = 700f;
    private Vector3 _runVectorForce2;

    Pose leftArmPose;
    Pose rightArmPose;
    Pose leftLegPose;
    Pose rightLegPose;


    
    


    public enum Side
    {
        Left = 0,
        Right = 1
    }

    public enum Pose
    {
        Bent = 0,
        Forward = 1,
        Straight = 2,
        Behind = 3
    }




    void Start()
    {
        bodyHandeler = GetComponent<BodyHandeler>();
    }

    private void FixedUpdate()
    {
        _moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (_moveInput.magnitude != 0)
        {
            if(!isMove)
            {
                isMove = true;
                if (Random.Range(0, 2) == 1)
                {
                    leftLegPose = Pose.Bent;
                    rightLegPose = Pose.Straight;
                    leftArmPose = Pose.Straight;
                    rightArmPose = Pose.Bent;
                }
                else
                {
                    leftLegPose = Pose.Straight;
                    rightLegPose = Pose.Bent;
                    leftArmPose = Pose.Bent;
                    rightArmPose = Pose.Straight;
                }
            }
            Move();
        }
        else
        {
            isMove = false;
        }


        
        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded)
            {
                _hips.AddForce(new Vector3(0,JumpForce,0));
                isGrounded = false;
            }
        }



    }



    private void Update()
    {
        CursorControll();
        LookAround();

        if (Input.GetKey(KeyCode.LeftShift))
            isRun = true;
        else
            isRun = false;

        //Ray_1();
    }


    #region 기즈모
    //private Ray ray;

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
    //}
    //void Ray_1()
    //{
    //    ray.origin = _hips.transform.position;
    //    ray.direction = -_hips.transform.right;
    //}
    #endregion


    private void RunCycleUpdate()
    {
        if (_cycleTimer < _cycleSpeed)
        {
            _cycleTimer += Time.deltaTime;
            return;
        }
        _cycleTimer = 0f;
        int num = (int)leftArmPose;
        num++;
        leftArmPose = ((num <= 3) ? ((Pose)num) : Pose.Bent);
        int num2 = (int)rightArmPose;
        num2++;
        rightArmPose = ((num2 <= 3) ? ((Pose)num2) : Pose.Bent);
        int num3 = (int)leftLegPose;
        num3++;
        leftLegPose = ((num3 <= 3) ? ((Pose)num3) : Pose.Bent);
        int num4 = (int)rightLegPose;
        num4++;
        rightLegPose = ((num4 <= 3) ? ((Pose)num4) : Pose.Bent);
    }


    
    private void RunCyclePoseLeg(Side side, Pose pose)
    {
        Transform hip =bodyHandeler.Hips.transform;
        Transform thighTrans = null;
        Transform legTrans = null;

        Rigidbody thighRigid = null;
        Rigidbody legRigid = null;
        Rigidbody footRigid = null;

        switch (side)
        {
            case Side.Left:
                thighTrans = bodyHandeler.LeftThigh.transform;
                legTrans = bodyHandeler.LeftLeg.transform;
                
                thighRigid = bodyHandeler.LeftThigh.GetComponent<Rigidbody>();
                legRigid = bodyHandeler.LeftLeg.PartRigidbody;
                footRigid = bodyHandeler.LeftFoot.PartRigidbody;

                //_ = actor.bodyHandeler.RightFoot.PartTransform;
                //_ = actor.bodyHandeler.RightThigh.PartRigidbody;

                break;
            case Side.Right:
                thighTrans = bodyHandeler.RightThigh.transform;
                legTrans = bodyHandeler.RightLeg.transform;
                //_ = actor.bodyHandeler.RightFoot.PartTransform;
                thighRigid = bodyHandeler.RightThigh.PartRigidbody;
                //_ = actor.bodyHandeler.LeftThigh.PartRigidbody;
                legRigid = bodyHandeler.RightLeg.PartRigidbody;
                footRigid = bodyHandeler.RightFoot.PartRigidbody;
                break;
        }
        if (isRun)
        {
            switch (pose)
            {
                case Pose.Bent:
                    AlignToVector(thighRigid, -thighTrans.forward,_moveDir, 0.1f, 3f * _applyedForce);
                    AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 3f * _applyedForce);
                    break;
                case Pose.Forward:
                    AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 3f * _applyedForce);
                    AlignToVector(legRigid, -legTrans.forward, _moveDir, 0.1f, 3f * _applyedForce);
                    if (!isDuck)
                    {
                        thighRigid.AddForce(-_moveDir * 2f * _applyedForce);
                        footRigid.AddForce(_moveDir * 2f * _applyedForce);
                    }
                    break;
                case Pose.Straight:
                    AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                    AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                    if (!isDuck)
                    {
                        thighRigid.AddForce(hip.up * 2f * _applyedForce);
                        footRigid.AddForce(-hip.up * 2f * _applyedForce);
                        footRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                    }
                    break;
                case Pose.Behind:
                    AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 3f * _applyedForce);
                    AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 3f * _applyedForce);
                    bodyHandeler.Hips.PartRigidbody.AddForce(_runVectorForce2 * _applyedForce, ForceMode.VelocityChange);
                    //bodyHandeler.Ball.PartRigidbody.AddForce(-_runVectorForce2 * _applyedForce, ForceMode.VelocityChange);
                    if (!isDuck)
                    {
                        footRigid.AddForce(-bodyHandeler.Hips.transform.forward, ForceMode.VelocityChange);
                    }
                    else
                    {
                        footRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                    }
                    break;
            }
            return;
        }
        switch (pose)
        {
            case Pose.Bent:
                AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
                break;
            case Pose.Forward:
                AlignToVector(thighRigid, -thighTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                if (!isDuck)
                {
                    thighRigid.AddForce(-_moveDir / 2f, ForceMode.VelocityChange);
                    footRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);
                }
                break;
            case Pose.Straight:
                AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                if (!isDuck)
                {
                    thighRigid.AddForce(hip.up * 2f * _applyedForce);
                    footRigid.AddForce(-hip.up * 2f * _applyedForce);
                    footRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                }
                break;
            case Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
                if (isDuck)
                {
                    bodyHandeler.Hips.PartRigidbody.AddForce(_runVectorForce2, ForceMode.VelocityChange);
                    //bodyHandeler.Ball.PartRigidbody.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                    footRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                }
                break;


        }
    }




    private void Move()
    {
        if (isRun)
        {
            _cycleSpeed = 0.15f;
        }
        else
        {
            _cycleSpeed = 0.2f;
        }



        RunCycleUpdate();

        RunCyclePoseBody();
        RunCyclePoseLeg(Side.Left, leftLegPose);
        RunCyclePoseLeg(Side.Right, rightLegPose);

 
    }

    private void RunCyclePoseBody()
    {
        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        _moveDir = lookForward * _moveInput.z + lookRight * _moveInput.x;


        AlignToVector(_hips, -_hips.transform.up, _moveDir, 0.1f, 4f * _rotationSpeed);

        //bodyHandeler.Chest.PartRigidbody.AddForce((runVectorForce10 + actor.controlHandeler.direction * runForce) * actor.applyedForce, ForceMode.VelocityChange);
        //bodyHandeler.Hips.PartRigidbody.AddForce((-runVectorForce5 + -(actor.controlHandeler.direction * runForce)) * actor.applyedForce, ForceMode.VelocityChange);
        //bodyHandeler.Ball.PartRigidbody.AddForce(-runVectorForce5 * actor.applyedForce, ForceMode.VelocityChange);
        //AlignToVector(bodyHandeler.Head.PartRigidbody, bodyHandeler.Head.PartTransform.forward, actor.controlHandeler.lookDirection, 0.1f, 2.5f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Head.PartRigidbody, bodyHandeler.Head.PartTransform.up, Vector3.up, 0.1f, 2.5f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Chest.PartRigidbody, bodyHandeler.Chest.transformforward, actor.controlHandeler.direction / 4f + -Vector3.up, 0.1f, 4f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Chest.PartRigidbody, bodyHandeler.Chest.transform.up, Vector3.up, 0.1f, 8f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Waist.PartRigidbody, bodyHandeler.Waist.transform.forward, actor.controlHandeler.direction / 4f + -Vector3.up, 0.1f, 4f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Waist.PartRigidbody, bodyHandeler.Chest.transform.up, Vector3.up, 0.1f, 8f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Hips.PartRigidbody, bodyHandeler.Hips.transform.forward, actor.controlHandeler.direction, 0.1f, 8f * actor.applyedForce);
        //AlignToVector(bodyHandeler.Hips.PartRigidbody, bodyHandeler.Hips.transform.up, Vector3.up, 0.1f, 8f * actor.applyedForce);




        if (isRun)
        {
            _hips.AddForce(_moveDir.normalized * Speed * Time.deltaTime * 2f);
            _cycleSpeed = 0.15f;
        }
        else
        {
            _hips.AddForce(_moveDir.normalized * Speed * Time.deltaTime);
            _cycleSpeed = 0.2f;
        }
    }


    //리지드바디 part의 alignmentVector방향을 targetVector방향으로 회전
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

    //카메라 컨트롤
    private void LookAround()
    {
        _cameraArm.parent.transform.position = _hips.transform.position;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }


    // 마우스 컨트롤 온오프
    private void CursorControll()
    {
        if (Input.anyKeyDown)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


    }
}
