using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/*
 * 신체부위들을 바로 접근할 수 있게 멤버로 보유하고 있는 클래스
 * 부위들의 관절을 고정하거나 해제시켜서 기절,아이템 장착 등의 모션을 구현하는 함수 보유
 */

public class BodyHandler : MonoBehaviourPun
{
    public Transform Root;
    public List<BodyPart> BodyParts = new List<BodyPart>(17);

    public BodyPart LeftFoot;
    public BodyPart RightFoot;
    public BodyPart LeftLeg;
    public BodyPart RightLeg;
    public BodyPart LeftThigh;
    public BodyPart RightThigh;

    public BodyPart Hip;
    public BodyPart Waist;
    public BodyPart Chest;
    public BodyPart Head;

    public BodyPart LeftArm;
    public BodyPart RightArm;
    public BodyPart LeftForeArm;
    public BodyPart RightForeArm;
    public BodyPart LeftHand;
    public BodyPart RightHand;

    public BodyPart Ball => GetBodyPart(Define.BodyPart.Ball);


    public ConfigurableJoint[] ChildJoints;
    public ConfigurableJointMotion[] OriginalYMotions;
    public ConfigurableJointMotion[] OriginalZMotions;
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    private FixedJoint[] _fixedArmJoints = new FixedJoint[6];
    private List<ConfigurableJoint> _chestArmJoints = new List<ConfigurableJoint>();

    const int _armJointCount = 3;
    const int _indexLeftStart = 1;
    const int _indexRightStart = 4;
    const int _indexChest = 0;
    const int _indexChangeDir = 3;

    private void Awake()
    {
        InitBodyParts();
        SaveConfigurableJoint();
    }

    private void InitBodyParts()
    {
        if (BodyParts.Count == 0)
        {
            foreach (Transform child in transform)
            {
                var component = child.GetComponent<BodyPart>();
                if (component != null)
                    BodyParts.Add(component);
            }
        }

        for (int i = 0; i < BodyParts.Count; i++)
            BodyParts[i].PartSetup((Define.BodyPart)i);
        

        LeftFoot = GetBodyPart(Define.BodyPart.FootL);
        RightFoot = GetBodyPart(Define.BodyPart.FootR);
        LeftLeg = GetBodyPart(Define.BodyPart.LegLowerL);
        RightLeg = GetBodyPart(Define.BodyPart.LegLowerR);
        LeftThigh = GetBodyPart(Define.BodyPart.LegUpperL);
        RightThigh = GetBodyPart(Define.BodyPart.LegUpperR);

        Hip = GetBodyPart(Define.BodyPart.Hip);
        Waist = GetBodyPart(Define.BodyPart.Waist);
        Chest = GetBodyPart(Define.BodyPart.Chest);
        Head = GetBodyPart(Define.BodyPart.Head);


        LeftArm = GetBodyPart(Define.BodyPart.LeftArm);
        RightArm = GetBodyPart(Define.BodyPart.RightArm);
        LeftForeArm = GetBodyPart(Define.BodyPart.LeftForeArm);
        RightForeArm = GetBodyPart(Define.BodyPart.RightForeArm);
        LeftHand = GetBodyPart(Define.BodyPart.LeftHand);
        RightHand = GetBodyPart(Define.BodyPart.RightHand);
    }

    //시작시 초기의 관절값들을 저장
    private void SaveConfigurableJoint()
    {
        ChildJoints = GetComponentsInChildren<ConfigurableJoint>();
        OriginalYMotions = new ConfigurableJointMotion[ChildJoints.Length];
        OriginalZMotions = new ConfigurableJointMotion[ChildJoints.Length];

        // 원래의 angularMotion 값을 저장
        for (int i = 0; i < ChildJoints.Length; i++)
        {
            OriginalYMotions[i] = ChildJoints[i].angularYMotion;
            OriginalZMotions[i] = ChildJoints[i].angularZMotion;
        }

        //초기 spring값 저장
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }

        //좌우 팔 관절 리스트 저장
        _chestArmJoints.Add(Chest.PartJoint);
        _chestArmJoints.Add(LeftArm.PartJoint);
        _chestArmJoints.Add(LeftForeArm.PartJoint);
        _chestArmJoints.Add(LeftHand.PartJoint);
        _chestArmJoints.Add(RightArm.PartJoint);
        _chestArmJoints.Add(RightForeArm.PartJoint);
        _chestArmJoints.Add(RightHand.PartJoint);
    }

    private BodyPart GetBodyPart(Define.BodyPart part)
    {
        return BodyParts[(int)part];
    }

    //관절의 스프링을 조절하는 함수
    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //기절과 회복에 모두 관여 기절시엔 퍼센티지를 0으로해서 사용
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }
    }

    public IEnumerator ResetBodySpring()
    {
        SetJointSpring(0f);
        yield return null;
    }

    //기절후 회복기능
    public IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time - startTime < springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            SetJointSpring(percentage);
            yield return null;
        }
    }

    //기절되서 스프링이 0이 된 관절을 그대로 고정시키는 함수
    public void JointLock(Define.Side side)
    {
        int start;
        if(side ==Define.Side.Left)
            start = _indexLeftStart;
        else
            start = _indexRightStart;

        for (int i = 0; i < _armJointCount; i++)
        {
            _chestArmJoints[start + i].angularYMotion = ConfigurableJointMotion.Locked;
            _chestArmJoints[start + i].angularZMotion = ConfigurableJointMotion.Locked;
        }
    }

    //손으로 잡고있어서 연결되어있던 관절을 해제
    public void DestroyJoint(FixedJoint right, FixedJoint left)
    {
        Destroy(left);
        Destroy(right);

        for (int i = 0; i < _armJointCount * 2; i++)
        {
            _chestArmJoints[_indexLeftStart + i].angularYMotion = ConfigurableJointMotion.Limited;
            _chestArmJoints[_indexLeftStart + i].angularZMotion = ConfigurableJointMotion.Limited;
        }
    }

    //아이템 장착시 팔이 흔들리지 않게 장착모션으로 고정
    public IEnumerator LockArmPosition()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _fixedArmJoints.Length; i++)
        {
            _fixedArmJoints[i] = _chestArmJoints[i + 1].AddComponent<FixedJoint>();

            if (i == _indexChangeDir)
                _fixedArmJoints[i].connectedBody = _chestArmJoints[_indexChest].GetComponent<Rigidbody>();
            else
                _fixedArmJoints[i].connectedBody = _chestArmJoints[i].GetComponent<Rigidbody>();
        }
    }

    //고정된 팔을 해제시키는 기능
    public void UnlockArmPosition()
    {
        for (int i = 0; i < 6; i++)
        {
            Debug.Log("UnlockArm" + _fixedArmJoints[i]);
            Destroy(_fixedArmJoints[i]);
            _fixedArmJoints[i] = null;
        }
    }

    public void ChangeDamageModifier(Define.BodyPart bodyPart, bool isAttack)
    {
        photonView.RPC(nameof(UpdateDamageModifier), RpcTarget.MasterClient, (int)bodyPart, true);
    }


    [PunRPC]
    private void UpdateDamageModifier(int bodyPart, bool isAttack)
    {
        //Debug.Log("[UpdateDamageModifier] isAttack: " + isAttack + ", bodyPart: " + bodyPart);

        switch ((Define.BodyPart)bodyPart)
        {
            case Define.BodyPart.FootL:
                if (isAttack)
                    this.LeftFoot.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this.LeftFoot.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.FootR:
                if (isAttack)
                    this.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LegLowerL:
                if (isAttack)
                    this.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LegLowerR:
                if (isAttack)
                    this.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LegUpperL:
                break;
            case Define.BodyPart.LegUpperR:
                break;
            case Define.BodyPart.Hip:
                break;
            case Define.BodyPart.Waist:
                break;
            case Define.BodyPart.Chest:
                break;
            case Define.BodyPart.Head:
                if (isAttack)
                    this.Head.PartInteractable.damageModifier = InteractableObject.Damage.Headbutt;
                else
                    this.Head.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LeftArm:
                break;
            case Define.BodyPart.RightArm:
                break;
            case Define.BodyPart.LeftForeArm:
                break;
            case Define.BodyPart.RightForeArm:
                break;
            case Define.BodyPart.LeftHand:
                if (isAttack)
                    this.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                else
                    this.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.RightHand:
                if (isAttack)
                    this.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                else
                    this.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
        }
    }
}

