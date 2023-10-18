using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("앞뒤 속도")]
    public float RunSpeed;
    [Header("점프 힘")]
    public float JumpForce;

    [SerializeField]
    private Rigidbody _hips;
    [SerializeField]
    private Transform _cameraArm;

    [SerializeField]
    private BodyHandeler bodyHandeler;

    [SerializeField]
    private TargetingHandeler targetingHandeler;

    public bool isGrounded;
    public bool isRun;
    public bool isMove;
    public bool isDuck;
    public bool isKickDuck;

    private Vector3 _moveInput;
    private Vector3 _moveDir;
    private float _rotationSpeed = 10f;



    private float _cycleTimer = 0;
    private float _cycleSpeed;
    private float _applyedForce = 800f;
    private Vector3 _runVectorForce2 = new Vector3(0f,0f,0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);
    private Vector3 _counterForce = new Vector3(0f, 0f, 0.4f);
    

    Pose leftArmPose;
    Pose rightArmPose;
    Pose leftLegPose;
    Pose rightLegPose;

    Side _readySide = Side.Left;

    InteractableObject target;
    
    


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
        targetingHandeler = GetComponent<TargetingHandeler>();

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.Input.KeyAction -= OnKeyBoardEvent;
        Managers.Input.KeyAction += OnKeyBoardEvent;

    }


    void OnKeyBoardEvent()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
       
    }


    void OnMouseEvent(Define.MouseEvent evt)
    {
        OnMouseEvent_Idle(evt);


        //상태별 마우스 이벤트 추가 예정
        //switch (State)
        //{
        //    case Define.State.Idle:
        //        OnMouseEvent_Idle(evt);
        //        break;
        //    case Define.State.Moving:
        //        OnMouseEvent_Idle(evt);
        //        break;
           
        //}
    }


    void OnMouseEvent_Idle(Define.MouseEvent evt)
    {

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {

                }
                break;
            case Define.MouseEvent.Press:
                {

                }
                break;
            case Define.MouseEvent.PointerUp:
                {

                }
                break;
            case Define.MouseEvent.Click:
                {
                    Punch();
                }
                break;
        }
    }




    private void FixedUpdate()
    {
        _moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (_moveInput.magnitude != 0)
        {
            if(!isMove)
            {
                isMove = true;
                if (UnityEngine.Random.Range(0, 2) == 1)
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


    private void OnCollisionEnter(Collision collision)
    {

    }

    private void Punch()
    {
        //마우스를 클릭한 순간 앞에 오브젝트를 탐색하고
        targetingHandeler.SearchTarget();

        //일정 시간내에 뗄 경우 펀치를 발사 이때 탐색한 오브젝트가 있을 경우 그 방향으로 펀치 발사
        if(_readySide == Side.Left)
        {
            ArmActionReadying(Side.Left);
            ArmActionPunching(Side.Left);
            ArmActionPunchResetting(Side.Left);
            _readySide = Side.Right;
        }
        else
        {
            ArmActionReadying(Side.Right);
            ArmActionPunching(Side.Right);
            ArmActionPunchResetting(Side.Right);

            _readySide = Side.Left;
        }


        //일정 시간내에 마우스를 떼지 않을 경우 두가지로 분기를 나눔
        //1.발견한 오브젝트가 있으면 그 방향으로 그랩
        //2.발견한 오브젝트가 없으면 아무것도 하지 않음
    }



    public void ArmActionReadying(Side side)
    {
        Transform partTransform = bodyHandeler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody part = null;
        Rigidbody part2 = null;
        Vector3 targetVector = Vector3.zero;
        switch (side)
        {
            case Side.Left:
                transform = bodyHandeler.LeftArm.transform;
                transform2 = bodyHandeler.LeftForarm.transform;
                part =bodyHandeler.LeftArm.PartRigidbody;
                part2 = bodyHandeler.LeftForarm.PartRigidbody;
                targetVector = bodyHandeler.Chest.transform.right;
                break;
            case Side.Right:
                transform = bodyHandeler.RightArm.transform;
                transform2 = bodyHandeler.RightForarm.transform;
                part = bodyHandeler.RightArm.PartRigidbody;
                part2 = bodyHandeler.RightForarm.PartRigidbody;
                targetVector = -bodyHandeler.Chest.transform.right;
                break;
        }
        AlignToVector(part, transform.up, targetVector, 0.01f, 20f);
        AlignToVector(part2, transform2.up, -partTransform.forward, 0.01f, 20f);

    }

    public void ArmActionPunching(Side side)
    {
        Transform partTransform = bodyHandeler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody rigidbody = null;
        Rigidbody rigidbody2 = null;
        switch (side)
        {
            case Side.Left:
                transform = bodyHandeler.LeftArm.transform;
                
                transform2 = bodyHandeler.LeftHand.transform;
                rigidbody = bodyHandeler.LeftArm.PartRigidbody;
                rigidbody2 = bodyHandeler.LeftHand.PartRigidbody;
                //bodyHandeler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                bodyHandeler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                bodyHandeler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                break;
            case Side.Right:
                transform = bodyHandeler.RightArm.transform;
              
                transform2 = bodyHandeler.RightHand.transform;
                rigidbody = bodyHandeler.RightArm.PartRigidbody;
                rigidbody2 = bodyHandeler.RightHand.PartRigidbody;
                //bodyHandeler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                bodyHandeler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                bodyHandeler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                break;
        }

        Vector3 zero = Vector3.zero;
        if (target == null)
        {
            zero = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
            rigidbody.AddForce(-(zero * 20f), ForceMode.VelocityChange);
            rigidbody2.AddForce(zero * 30f, ForceMode.VelocityChange);
            return;
        }

        //타겟있을때

        //zero = Vector3.Normalize(target.cachedCollider.ClosestPoint(transform2.position) - transform2.position);
        //if (Vector3.Distance(targetingHandeler.upperIntrest.cachedCollider.bounds.center, transform.position) > 1.2f)
        //{

        //    rigidbody.AddForce(-(zero * 20f), ForceMode.VelocityChange);
        //    rigidbody2.AddForce(zero * 40f, ForceMode.VelocityChange);
        //}
        //else
        //{
        //    rigidbody.AddForce(-(zero * 20f), ForceMode.VelocityChange);
        //    rigidbody2.AddForce(zero * 50f, ForceMode.VelocityChange);
        //}
    }

    public void ArmActionPunchResetting(Side side)
    {
        Transform partTransform = bodyHandeler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody part = null;
        Rigidbody part2 = null;
        Vector3 vector = Vector3.zero;
        switch (side)
        {
            case Side.Left:
                transform = bodyHandeler.LeftArm.transform;
                transform2 = bodyHandeler.LeftForarm.transform;
                part = bodyHandeler.LeftArm.PartRigidbody;
                part2 = bodyHandeler.LeftForarm.PartRigidbody;
                vector = bodyHandeler.Chest.transform.right / 2f;
                //bodyHandeler.LeftArm.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                //bodyHandeler.LeftForarm.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                //bodyHandeler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                bodyHandeler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                bodyHandeler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                break;
            case Side.Right:
                transform = bodyHandeler.RightArm.transform;
                transform2 = bodyHandeler.RightForarm.transform;
                part = bodyHandeler.RightArm.PartRigidbody;
                part2 = bodyHandeler.RightForarm.PartRigidbody;
                vector = -bodyHandeler.Chest.transform.right / 2f;
                //bodyHandeler.RightArm.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                //bodyHandeler.RightForarm.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                //bodyHandeler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                bodyHandeler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                bodyHandeler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                break;
        }
        AlignToVector(part, transform.forward, vector + -partTransform.up, 0.01f, 2f);
        AlignToVector(part2, transform2.forward, vector + partTransform.up, 0.01f, 2f);
    }


    private void Jump()
    {
        isGrounded = false;


        float jumpMoveForce = 0.05f;

        //움직이는 입력이 있을때
        if (_moveDir != Vector3.zero)
        {
            bodyHandeler.Chest.PartRigidbody.AddForce(_moveDir * jumpMoveForce, ForceMode.VelocityChange);
            bodyHandeler.Waist.PartRigidbody.AddForce(_moveDir * jumpMoveForce, ForceMode.VelocityChange);
            bodyHandeler.Hip.PartRigidbody.AddForce(_moveDir * jumpMoveForce, ForceMode.VelocityChange);
        }
        AlignToVector(bodyHandeler.Head.PartRigidbody, -bodyHandeler.Head.transform.up, _moveDir + new Vector3(0,0.2f,0f), 0.1f, 4f);
        AlignToVector(bodyHandeler.Head.PartRigidbody, bodyHandeler.Head.transform.forward, Vector3.up, 0.1f, 4f);


        //일반점프
        if (!isDuck && !isKickDuck)
        {
            bodyHandeler.Chest.PartRigidbody.AddForce(Vector3.up * 1.7f * JumpForce, ForceMode.VelocityChange);
            bodyHandeler.Hip.PartRigidbody.AddForce(Vector3.down * 1.7f * JumpForce, ForceMode.VelocityChange);
            AlignToVector(bodyHandeler.Chest.PartRigidbody, -bodyHandeler.Chest.transform.up,_moveDir+ new Vector3(0f, -1f, 0f), 0.1f, 10f);
            AlignToVector(bodyHandeler.Waist.PartRigidbody, -bodyHandeler.Waist.transform.up, _moveDir + new Vector3(0f, -0f, 0f), 0.1f, 10f);
            AlignToVector(bodyHandeler.Hip.PartRigidbody, -bodyHandeler.Hip.transform.up, _moveDir + new Vector3(0f, 1f, 0f), 0.1f, 10f);
            
            //왼팔이 사용중이 아닐때
            if (true)
            {
                AlignToVector(bodyHandeler.LeftArm.PartRigidbody,bodyHandeler.LeftArm.transform.forward, (bodyHandeler.Chest.transform.right + -bodyHandeler.Chest.transform.up) / 4f, 0.1f, 4f);
                AlignToVector(bodyHandeler.LeftForarm.PartRigidbody, bodyHandeler.LeftForarm.transform.forward, (bodyHandeler.Chest.transform.right + bodyHandeler.Chest.transform.up) / 4f, 0.1f, 4f);
            }
            //오른팔이 사용중이 아닐때
            if (true)
            {
                AlignToVector(bodyHandeler.RightArm.PartRigidbody, bodyHandeler.RightArm.transform.forward, (-bodyHandeler.Chest.transform.right + -bodyHandeler.Chest.transform.up) / 4f, 0.1f, 4f);
                AlignToVector(bodyHandeler.RightForarm.PartRigidbody,bodyHandeler.RightForarm.transform.forward, (-bodyHandeler.Chest.transform.right + bodyHandeler.Chest.transform.up) / 4f, 0.1f, 4f);
            }
            AlignToVector(bodyHandeler.LeftThigh.PartRigidbody, bodyHandeler.LeftThigh.transform.forward, bodyHandeler.Hip.transform.forward + bodyHandeler.Hip.transform.up, 0.1f, 4f);
            AlignToVector(bodyHandeler.LeftLeg.PartRigidbody,bodyHandeler.LeftLeg.transform.forward,bodyHandeler.Hip.transform.forward + -bodyHandeler.Hip.transform.up, 0.1f, 4f);
            AlignToVector(bodyHandeler.RightThigh.PartRigidbody, bodyHandeler.RightThigh.transform.forward, bodyHandeler.Hip.transform.forward + bodyHandeler.Hip.transform.up, 0.1f, 4f);
            AlignToVector(bodyHandeler.RightLeg.PartRigidbody, bodyHandeler.RightLeg.transform.forward, bodyHandeler.Hip.transform.forward + -bodyHandeler.Hip.transform.up, 0.1f, 4f);
        }
    }

    private void Move()
    {
        if (isRun)
        {
            _cycleSpeed = 0.1f;
        }
        else
        {
            _cycleSpeed = 0.15f;
        }



        RunCycleUpdate();

        RunCyclePoseBody();
        RunCyclePoseArm(Side.Left, leftArmPose);
        RunCyclePoseArm(Side.Right, rightArmPose);

        RunCyclePoseLeg(Side.Left, leftLegPose);
        RunCyclePoseLeg(Side.Right, rightLegPose);


    }

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
        Transform hip =bodyHandeler.Hip.transform;
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
                    footRigid.AddForce(-_runVectorForce2 , ForceMode.VelocityChange);
                }
                break;
            case Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
                if (isDuck)
                {
                    bodyHandeler.Hip.PartRigidbody.AddForce(_runVectorForce2, ForceMode.VelocityChange);
                    bodyHandeler.Ball.PartRigidbody.AddForce(-_runVectorForce2 , ForceMode.VelocityChange);
                    footRigid.AddForce(-_runVectorForce2 , ForceMode.VelocityChange);
                }
                break;


        }
    }


    private void RunCyclePoseArm(Side side, Pose pose)
    {
        Vector3 vector = Vector3.zero;
        Transform partTransform = bodyHandeler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody rigidbody = null;
        Rigidbody rigidbody2 = null;
        Rigidbody rigidbody3 = null;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;


        switch (side)
        {
            case Side.Left:
                transform = bodyHandeler.LeftArm.transform;
                transform2 = bodyHandeler.LeftForarm.transform;
                rigidbody = bodyHandeler.LeftArm.PartRigidbody;
                rigidbody2 = bodyHandeler.LeftForarm.PartRigidbody;
                rigidbody3 = bodyHandeler.LeftHand.PartRigidbody;
                vector = bodyHandeler.Chest.transform.right;
                break;
            case Side.Right:
                transform = bodyHandeler.RightArm.transform;
                transform2 = bodyHandeler.RightForarm.transform;
                rigidbody = bodyHandeler.RightArm.PartRigidbody;
                rigidbody2 = bodyHandeler.RightForarm.PartRigidbody;
                rigidbody3 = bodyHandeler.RightHand.PartRigidbody;
                vector = -bodyHandeler.Chest.transform.right;
                break;
        }
        if (!isDuck && !isKickDuck && !isRun)
        {
            switch (pose)
            {
                case Pose.Bent:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -_moveDir / 4f, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case Pose.Forward:
                    AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, _moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case Pose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case Pose.Behind:
                    AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
            }
            return;
        }
        switch (pose)
        {
            case Pose.Bent:
                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, -_moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Pose.Forward:
                AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce);
                AlignToVector(rigidbody2, transform2.forward, _moveDir + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Pose.Straight:
                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Pose.Behind:
                AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
        }
    }


    private void RunCyclePoseBody()
    {
        
        //if (isRun)
        //{
        //    RunSpeed = Mathf.Clamp(RunSpeed += Time.deltaTime, 0f, 1f);
        //}
        //else
        //{
        //    RunSpeed = Mathf.Clamp(RunSpeed -= Time.deltaTime, 0f, 1f);
        //}


        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        _moveDir = lookForward * _moveInput.z + lookRight * _moveInput.x;


        //AlignToVector(_hips, -_hips.transform.up, _moveDir, 0.1f, 4f * _rotationSpeed);

        bodyHandeler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir) , ForceMode.VelocityChange);
        bodyHandeler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);
        //bodyHandeler.Ball.PartRigidbody.AddForce(-_runVectorForce5 * _applyedForce, ForceMode.VelocityChange);

        //AlignToVector(bodyHandeler.Head.PartRigidbody, -bodyHandeler.Head.transform.up, _moveDir, 0.1f, 2.5f * _applyedForce);
        //AlignToVector(bodyHandeler.Head.PartRigidbody, bodyHandeler.Head.transform.forward, _moveDir + new Vector3(0,0.2f,0f), 0.1f, 2.5f * _applyedForce);

       //AlignToVector(bodyHandeler.Head.PartRigidbody, bodyHandeler.Head.transform.forward, Vector3.up, 0.1f, 2.5f * _applyedForce);
        AlignToVector(bodyHandeler.Chest.PartRigidbody, -bodyHandeler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(bodyHandeler.Chest.PartRigidbody, bodyHandeler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(bodyHandeler.Waist.PartRigidbody, -bodyHandeler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(bodyHandeler.Waist.PartRigidbody, bodyHandeler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(bodyHandeler.Hip.PartRigidbody, -bodyHandeler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        AlignToVector(bodyHandeler.Hip.PartRigidbody, bodyHandeler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);




        if (isRun)
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * Time.deltaTime * 1.5f);
        }
        else
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * Time.deltaTime);
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
