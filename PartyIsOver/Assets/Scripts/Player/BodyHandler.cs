using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/*
 * ��ü�������� �ٷ� ������ �� �ְ� ����� �����ϰ� �ִ� Ŭ����
 * �������� ������ �����ϰų� �������Ѽ� ����,������ ���� ���� ����� �����ϴ� �Լ� ����
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

    //���۽� �ʱ��� ���������� ����
    private void SaveConfigurableJoint()
    {
        ChildJoints = GetComponentsInChildren<ConfigurableJoint>();
        OriginalYMotions = new ConfigurableJointMotion[ChildJoints.Length];
        OriginalZMotions = new ConfigurableJointMotion[ChildJoints.Length];

        // ������ angularMotion ���� ����
        for (int i = 0; i < ChildJoints.Length; i++)
        {
            OriginalYMotions[i] = ChildJoints[i].angularYMotion;
            OriginalZMotions[i] = ChildJoints[i].angularZMotion;
        }

        //�ʱ� spring�� ����
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }

        //�¿� �� ���� ����Ʈ ����
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

    //������ �������� �����ϴ� �Լ�
    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //������ ȸ���� ��� ���� �����ÿ� �ۼ�Ƽ���� 0�����ؼ� ���
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

    //������ ȸ�����
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

    //�����Ǽ� �������� 0�� �� ������ �״�� ������Ű�� �Լ�
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

    //������ ����־ ����Ǿ��ִ� ������ ����
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

    //������ ������ ���� ��鸮�� �ʰ� ����������� ����
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

    //������ ���� ������Ű�� ���
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

