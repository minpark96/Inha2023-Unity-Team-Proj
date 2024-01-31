using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction
{
    PlayerActions _actions;
    //AnimationData _animData;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;

    Vector3 _moveDir = new Vector3();

    private float _runSpeed;
    private float _maxSpeed;
    private bool _isRun;

    private float _runSpeedOffset = 350f;
    private float _applyedForce = 800f;

    private Vector3 _runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);

    int[] limbPositions = new int[4];

    public MoveAction(PlayerActions actions)
    {
        _actions = actions;
        _actions.OnMove -= InvokeMoveEvent;
        _actions.OnMove += InvokeMoveEvent;

        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");
        _maxSpeed = statData.MaxSpeed;
        _runSpeed = statData.RunSpeed;
    }


    public void InvokeMoveEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyhandler, in Define.PlayerDynamicData data)
    {
        //_animData = animData;
        _animPlayer = animPlayer;
        _bodyHandler = bodyhandler;

        _moveDir.x = data.dirX;
        _moveDir.y = data.dirY;
        _moveDir.z = data.dirZ;
        _isRun = data.isRunState;

        if (_isRun)
            Debug.Log("Run");
        for (int i = 0; i < (int)Define.limbPositions.End; i++) 
        {
            limbPositions[i] = data.limbPositions[i];
        }
        
        if (data.isGrounded)
            MovePlayer();
        else
            InAirMove();
    }

    private void InAirMove()
    {
        _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        _animPlayer.AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

        //Fall상태로 빼야 할수도
        _bodyHandler.Hip.PartRigidbody.AddForce(_moveDir.normalized * _runSpeed * _runSpeedOffset * Time.deltaTime * 0.5f);

        if (_bodyHandler.Hip.PartRigidbody.velocity.magnitude > _maxSpeed)
            _bodyHandler.Hip.PartRigidbody.velocity = _bodyHandler.Hip.PartRigidbody.velocity.normalized * _maxSpeed;
    }

    private void MovePlayer()
    {
        RunCyclePoseBody();
        RunCyclePoseArm(Define.Side.Left, limbPositions[(int)Define.limbPositions.leftArmPose]);
        RunCyclePoseArm(Define.Side.Right, limbPositions[(int)Define.limbPositions.rightArmPose]);
        RunCyclePoseLeg(Define.Side.Left, limbPositions[(int)Define.limbPositions.leftLegPose]);
        RunCyclePoseLeg(Define.Side.Right, limbPositions[(int)Define.limbPositions.rightLegPose]);
    }

    private void RunCyclePoseLeg(Define.Side side, int limbPose)
    {
        Transform hip = _bodyHandler.Hip.transform;
        Transform thighTrans = null;
        Transform legTrans = null;

        Rigidbody thighRigid = null;
        Rigidbody legRigid = null;

        switch (side)
        {
            case Define.Side.Left:
                thighTrans = _bodyHandler.LeftThigh.transform;
                legTrans = _bodyHandler.LeftLeg.transform;

                thighRigid = _bodyHandler.LeftThigh.GetComponent<Rigidbody>();
                legRigid = _bodyHandler.LeftLeg.PartRigidbody;
                break;
            case Define.Side.Right:
                thighTrans = _bodyHandler.RightThigh.transform;
                legTrans = _bodyHandler.RightLeg.transform;
                thighRigid = _bodyHandler.RightThigh.PartRigidbody;
                legRigid = _bodyHandler.RightLeg.PartRigidbody;
                break;
        }

        switch (limbPose)
        {
            case (int)Define.BodyPose.Bent:
                _animPlayer.AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
                _animPlayer.AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
                break;
            case (int)Define.BodyPose.Forward:
                _animPlayer.AlignToVector(thighRigid, -thighTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                _animPlayer.AlignToVector(legRigid, -legTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                thighRigid.AddForce(-_moveDir / 2f, ForceMode.VelocityChange);
                legRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);

                break;
            case (int)Define.BodyPose.Straight:
                _animPlayer.AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                _animPlayer.AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                thighRigid.AddForce(hip.up * 2f * _applyedForce);
                legRigid.AddForce(-hip.up * 2f * _applyedForce);
                legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);

                break;
            case (int)Define.BodyPose.Behind:
                _animPlayer.AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
                _animPlayer.AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
                break;
        }
    }

    private void RunCyclePoseArm(Define.Side side, int limbPose)
    {
        Vector3 vector = Vector3.zero;
        Transform partTransform = _bodyHandler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody rigidbody = null;
        Rigidbody rigidbody2 = null;
        Rigidbody rigidbody3 = null;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;
        switch (side)
        {
            case Define.Side.Left:
                transform = _bodyHandler.LeftArm.transform;
                transform2 = _bodyHandler.LeftForeArm.transform;
                rigidbody = _bodyHandler.LeftArm.PartRigidbody;
                rigidbody2 = _bodyHandler.LeftForeArm.PartRigidbody;
                rigidbody3 = _bodyHandler.LeftHand.PartRigidbody;
                vector = _bodyHandler.Chest.transform.right;
                break;
            case Define.Side.Right:
                transform = _bodyHandler.RightArm.transform;
                transform2 = _bodyHandler.RightForeArm.transform;
                rigidbody = _bodyHandler.RightArm.PartRigidbody;
                rigidbody2 = _bodyHandler.RightForeArm.PartRigidbody;
                rigidbody3 = _bodyHandler.RightHand.PartRigidbody;
                vector = -_bodyHandler.Chest.transform.right;
                break;
        }
        if (!_isRun)
        {
            switch (limbPose)
            {
                case (int)Define.BodyPose.Bent:
                    _animPlayer.AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    _animPlayer.AlignToVector(rigidbody2, transform2.forward, -_moveDir / 4f, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case (int)Define.BodyPose.Forward:
                    _animPlayer.AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    _animPlayer.AlignToVector(rigidbody2, transform2.forward, _moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case (int)Define.BodyPose.Straight:
                    _animPlayer.AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    _animPlayer.AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case (int)Define.BodyPose.Behind:
                    _animPlayer.AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                    _animPlayer.AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
            }
            return;
        }
        switch (limbPose)
        {
            case (int)Define.BodyPose.Bent:
                _animPlayer.AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                _animPlayer.AlignToVector(rigidbody2, transform2.forward, -_moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case (int)Define.BodyPose.Forward:
                _animPlayer.AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce);
                _animPlayer.AlignToVector(rigidbody2, transform2.forward, _moveDir + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case (int)Define.BodyPose.Straight:
                _animPlayer.AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                _animPlayer.AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case (int)Define.BodyPose.Behind:
                _animPlayer.AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                _animPlayer.AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
        }
    }


    private void RunCyclePoseBody()
    {
        _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        _animPlayer.AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        _animPlayer.AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

        if (_isRun)
        {
            _bodyHandler.Hip.PartRigidbody.AddForce(_moveDir.normalized * _runSpeed * _runSpeedOffset * Time.deltaTime * 1.35f);
            if (_bodyHandler.Hip.PartRigidbody.velocity.magnitude > _maxSpeed)
                _bodyHandler.Hip.PartRigidbody.velocity = _bodyHandler.Hip.PartRigidbody.velocity.normalized * _maxSpeed * 1.15f;
        }
        else
        {
            _bodyHandler.Hip.PartRigidbody.AddForce(_moveDir.normalized * _runSpeed * _runSpeedOffset * Time.deltaTime);
            if (_bodyHandler.Hip.PartRigidbody.velocity.magnitude > _maxSpeed)
                _bodyHandler.Hip.PartRigidbody.velocity = _bodyHandler.Hip.PartRigidbody.velocity.normalized * _maxSpeed;
        }

    }
}
