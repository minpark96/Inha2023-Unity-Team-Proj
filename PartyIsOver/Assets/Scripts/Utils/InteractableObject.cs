using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class InteractableObject : MonoBehaviourPun
{
    Damage _initialDamage;
    public Damage damageModifier = Damage.Default;

    public Define.ObjectType Type;
    public Rigidbody RigidbodyObject;
    public Item ItemObject;
    public Collider ColliderObject;
    public PhotonView PhotonView;
    public int ViewID = -1;

    private void Start()
    {
        Init();
    }

    public enum Damage
    {
        Ignore = 0,
        Default = 1,
        Object = 2,
        Punch = 3,
        DropKick = 4,
        Headbutt = 5,
        NuclearPunch = 6,
        MeowNyangPunch= 7,

        PowerUp = 8,
        Burn = 9,
        Slow = 10,
        Ice = 11,
        Shock = 12,
        Stun = 13,
        Drunk = 14,
        Balloon = 15,
    }


    public void Init()
    {
        _initialDamage = damageModifier;

        ItemObject = GetComponent<Item>();
        ColliderObject = GetComponent<Collider>();
        RigidbodyObject = GetComponent<Rigidbody>();

        PhotonView = GetComponent<PhotonView>();
        if (PhotonView != null) ViewID = PhotonView.ViewID;

        if (ItemObject != null)
            Type = Define.ObjectType.Item;
        else if (GetComponent<BodyPart>() != null)
            Type = Define.ObjectType.Player;
        else if (gameObject.layer == (int)Define.Layer.ClimbObject)
            Type = Define.ObjectType.Wall;
        else
            Type = Define.ObjectType.Object;
    }

    public void PullingForceTrigger(Vector3 dir ,float power)
    {
        photonView.RPC("ApplyPullingForce", RpcTarget.All, dir, power);
    }

    [PunRPC]
    private void ApplyPullingForce(Vector3 dir, float power)
    {
        if (!photonView.IsMine)
            return;
        //Vector3 force = (vel - _rb.velocity) * _rb.mass; // �ӵ� ���̿� ������ ���Ͽ� ���� ���
        //_rb.AddForce(force, ForceMode.VelocityChange); // Impulse ��带 ����Ͽ� ���������� ���� ����
        RigidbodyObject.AddForce(Vector3.ClampMagnitude(dir.normalized * power, 100f), ForceMode.VelocityChange);
    }

    [PunRPC]
    public void ChangeUseTypeTrigger(float waitTime, float useTime)
    {
        StartCoroutine(ChangeUseType(waitTime, useTime));
    }
    IEnumerator ChangeUseType(float waitTime, float useTime)
    {
       
        yield return new WaitForSeconds(waitTime);

        if (GetComponent<Item>() != null)
        {
            damageModifier = GetComponent<Item>().ItemData.UseDamageType;
        }
        else if(GetComponent<ProjectileStandard>() != null)
        {
            if(GetComponent<ProjectileStandard>().Gun == null)
            {
                GetComponent<ProjectileStandard>().Gun = PhotonNetwork.GetPhotonView((int)photonView.InstantiationData[0]).GetComponent<Item>();
            }
            damageModifier = GetComponent<ProjectileStandard>().Gun.GetComponent<Item>().ItemData.UseDamageType;
        }
        else
        {
            damageModifier = Damage.Object;
        }
        if(useTime != -1)
        {
            yield return new WaitForSeconds(useTime);
            ResetType();
        }
    }

    public void ResetType()
    {
        damageModifier = _initialDamage;
    }
}
