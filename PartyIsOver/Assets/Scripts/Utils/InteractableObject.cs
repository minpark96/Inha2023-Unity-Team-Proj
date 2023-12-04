using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class InteractableObject : MonoBehaviourPun
{
    Rigidbody _rb;
    public Damage damageModifier = Damage.Default;
    Damage _initialDamage;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _initialDamage = damageModifier;
    }


    public enum Damage
    {
        Ignore = 0,
        Default = 1,
        Object = 2,
        Punch = 3,
        DropKick = 4,
        Headbutt = 5,
        Knockout = 6,
        Special= 7,

        PowerUp = 8,
        Burn = 9,
        Slow = 10,
        Ice = 11,
        Shock = 12,
        Stun = 13,
        Drunk = 14,
        Balloon = 15,
    }



    public void PullingForceTrigger(Vector3 dir ,float power)
    {
        photonView.RPC("ApplyPullingForce", RpcTarget.All, dir, power);
    }

    [PunRPC]
    public void ApplyPullingForce(Vector3 dir, float power)
    {
        if (!photonView.IsMine)
            return;
        //Vector3 force = (vel - _rb.velocity) * _rb.mass; // 속도 차이에 질량을 곱하여 힘을 계산
        //_rb.AddForce(force, ForceMode.VelocityChange); // Impulse 모드를 사용하여 순간적으로 힘을 적용
        _rb.AddForce(Vector3.ClampMagnitude(dir.normalized * power, 100f), ForceMode.VelocityChange);
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
            damageModifier = GetComponent<ProjectileStandard>().Gun.GetComponent<Item>().ItemData.UseDamageType;
        }
        else
        {
            damageModifier = Damage.Object;
        }

        yield return new WaitForSeconds(useTime);
        ResetType();
    }

    public void ResetType()
    {
        damageModifier = _initialDamage;
    }
}
