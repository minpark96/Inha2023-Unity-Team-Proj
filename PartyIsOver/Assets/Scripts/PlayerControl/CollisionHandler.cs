using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static InteractableObject;

public class CollisionHandler : MonoBehaviourPun
{
    public float damageMinimumVelocity = 0.25f;

    public Actor actor;
    private Transform rootTransform;

    [SerializeField] private float _objectDamage;
    [SerializeField] private float _punchDamage;
    [SerializeField] private float _dropkickDamage;
    [SerializeField] private float _headbuttDamage;

    [SerializeField] private float _objectForceNormal;
    [SerializeField] private float _punchForceNormal;
    [SerializeField] private float _dropkickForceNormal;
    [SerializeField] private float _headbuttForceNormal;

    [SerializeField] private float _objectForceUp;
    [SerializeField] private float _punchForceUp;
    [SerializeField] private float _headbuttForceUp;
    [SerializeField] private float _dropkickForceUp;

    [SerializeField] private float _headMultiple = 1.5f;
    [SerializeField] private float _armMultiple = 0.8f;
    [SerializeField] private float _handMultiple = 0.8f;
    [SerializeField] private float _legMultiple = 0.8f;

    void Start()
    {
        if (actor == null)
        {
            rootTransform = transform.root;
            actor = rootTransform.GetComponent<Actor>();
        }
        else
        {
            rootTransform = actor.transform;
        }
        Init();
    }

    void Init()
    {
        CollisionData data = Managers.Resource.Load<CollisionData>("ScriptableObject/CollisionData");
        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");
        _objectDamage = data._ObjectDamage;
        _punchDamage = data.PunchDamage;
        _dropkickDamage = data.DropkickDamage;
        _headbuttDamage = data.HeadbuttDamage;

        _objectForceNormal = data.ObjectForceNormal;
        _punchForceNormal = data.PunchForceNormal;
        _dropkickForceNormal = data.DropkickForceNormal;
        _headbuttForceNormal = data.HeadbuttForceNormal;

        _objectForceUp = data.ObjectForceUp;
        _punchForceUp = data.PunchForceUp;
        _headbuttForceUp = data.HeadbuttForceUp;
        _dropkickForceUp = data.DropkickForceUp;

        _headMultiple = statData.HeadMultiple;
        _armMultiple = statData.ArmMultiple;    
        _handMultiple = statData.HandMultiple;
        _legMultiple = statData.LegMultiple;

    }

    private void DamageCheck(Collision collision)
    {
        InteractableObject collisionInteractable = collision.transform.GetComponent<InteractableObject>();
        if (collisionInteractable == null)
            return;
        if (collision.gameObject.GetComponent<Item>() != null && collision.gameObject.GetComponent<Item>().Owner == actor)
            return;

        Rigidbody collisionRigidbody = collision.rigidbody;
        Collider collisionCollider = collision.collider;
        Vector3 relativeVelocity = collision.relativeVelocity;
        float velocityMagnitude = relativeVelocity.magnitude;

        float num = 0f;
        float damage = 0f;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);

            //충돌한 녀석이 rigidbody가 있으면 그 객체의 mass를 넣고 없으면 디폴트로 40을 넣음
            num = ((!collisionRigidbody) ? 40f : collisionRigidbody.mass);

            //충돌지점의 노멀벡터랑 relativeVelocity(충돌시 두 물체간의 상대적인 이동속도)를 곱하고 변환
            damage = Vector3.Dot(contact.normal, relativeVelocity) * Mathf.Clamp(num, 0f, 40f) / 1100f;

            //음수면 양수로 바꿈
            if (damage < 0f)
            {
                damage = 0f - damage;
            }

            // 물리적 공격을 받을 때
            if (collisionInteractable.damageModifier <= InteractableObject.Damage.Special)
            {
                damage = PhysicalDamage(collisionInteractable, damage, contact);
                damage = ApplyBodyPartDamageModifier(damage);
                damage *= actor.PlayerAttackPoint;
                damage = Mathf.RoundToInt(damage);
                damage -= damage * (actor.DamageReduction / 100f);

                // 데미지 적용
                if (damage > 0f && velocityMagnitude > damageMinimumVelocity)
                {
                    if (collisionInteractable != null)
                    {
                        actor.StatusHandler.AddDamage(collisionInteractable.damageModifier, damage, collisionCollider.gameObject);
                    }
                }
            }
            // 버프형 공격을 받을 때
            else
            {
                damage = 0;
                if (collisionInteractable != null)
                {
                    actor.StatusHandler.AddDamage(collisionInteractable.damageModifier, damage, collisionCollider.gameObject);
                }
            }
        }
    }

    private float ApplyBodyPartDamageModifier(float damage)
    {
        if (transform == actor.BodyHandler.RightArm.transform ||
            transform == actor.BodyHandler.LeftArm.transform)
            damage *= _armMultiple;
        else if (transform == actor.BodyHandler.RightForearm.transform ||
            transform == actor.BodyHandler.LeftForearm.transform)
            damage *= _armMultiple;
        else if (transform == actor.BodyHandler.RightHand.transform ||
            transform == actor.BodyHandler.LeftHand.transform)
            damage *= _handMultiple;
        else if (transform == actor.BodyHandler.RightLeg.transform ||
            transform == actor.BodyHandler.LeftLeg.transform)
            damage *=_legMultiple;
        else if (transform == actor.BodyHandler.RightThigh.transform ||
            transform == actor.BodyHandler.LeftThigh.transform)
            damage *= _legMultiple;
        //else if (transform == actor.BodyHandler.RightFoot.transform ||
            //transform == actor.BodyHandler.LeftFoot.transform)
            //damage *= actor.LegMultiple;
        else if (transform == actor.BodyHandler.Head.transform)
            damage *= _headMultiple;

        return damage;
    }

private float PhysicalDamage(InteractableObject collisionInteractable, float damage, ContactPoint contact)
    {
        float itemDamage = 100f;
        if (collisionInteractable.GetComponent<Item>() != null)
            itemDamage = collisionInteractable.GetComponent<Item>().ItemData.Damage;

        switch (collisionInteractable.damageModifier)
        {
            case InteractableObject.Damage.Ignore:
                damage = 0f;
                break;
            case InteractableObject.Damage.Object:
                {
                    //Rigidbody rb;
                    //if (collisionInteractable.GetComponent<Item>() != null)
                    //{
                    //    rb = collisionInteractable.GetComponent<Rigidbody>();
                    //    rb.velocity = Vector3.zero;
                    //    rb.angularVelocity = Vector3.zero;
                    //}
                    damage *= _objectDamage * itemDamage;
                    damage = Mathf.Clamp(damage, 15f, 50f);
                    actor.PlayerController.PlayerEffectSound("PlayerEffect/WEAPON_CrossBow");
                }
                break;
            case InteractableObject.Damage.Punch:
                damage *= _punchDamage;
                {
                    actor.PlayerController.PlayerEffectSound("PlayerEffect/SFX_ArrowShot_Hit");
                }
                break;
            case InteractableObject.Damage.DropKick:
                damage *= _dropkickDamage;
                {
                    actor.PlayerController.PlayerEffectSound("PlayerEffect/DAMAGE_Monster_01");
                }
                break;
            case InteractableObject.Damage.Headbutt:
                damage *= _headbuttDamage;
                {
                    actor.PlayerController.PlayerEffectSound("PlayerEffect/WEAPON_CrossBow");
                }
                break;
            case InteractableObject.Damage.Knockout:
                damage = 1000000f;
                break;
            default:
                break;
        }

        itemDamage = 1f;
        if (collisionInteractable.GetComponent<Item>() != null)
        {
            itemDamage = collisionInteractable.GetComponent<Item>().ItemData.Damage / 10f;
            if (itemDamage < 1f)
                itemDamage = 1f;
        }

        int thisViewID;
        if (contact.thisCollider.gameObject.GetComponent<PhotonView>() != null)
        {
            thisViewID = contact.thisCollider.gameObject.GetComponent<PhotonView>().ViewID;
            photonView.RPC("AddForceAttackedTarget", RpcTarget.All, thisViewID, NormalChange(contact.normal), (int)collisionInteractable.damageModifier, itemDamage);
        }

        damage = Mathf.Clamp(damage, 0f, 25f);
        return damage;
    }

    Vector3 NormalChange(Vector3 normal)
    {
        Vector3 newNormal = new Vector3(normal.x, normal.y / 3, normal.z);

        return newNormal.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected == true) return;

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Ground") && !actor.StatusHandler.invulnerable)
            DamageCheck(collision);
    }
  

    [PunRPC]
    void AddForceAttackedTarget(int objViewId, Vector3 normal, int damageModifier, float itemDamage)
    {
        //Debug.Log("[AddForceAttackedTarget] id: " + objViewId);
        Rigidbody thisRb = PhotonNetwork.GetPhotonView(objViewId).transform.GetComponent<Rigidbody>();

        Rigidbody body = null;
        if (thisRb.transform.GetComponent<BodyPart>() != null)
        {
            body = thisRb.transform.root.GetComponent<BodyHandler>().Hip.PartRigidbody;
        }

        switch ((InteractableObject.Damage)damageModifier)
        {
            case InteractableObject.Damage.Ignore:
                break;
            case InteractableObject.Damage.Object:
                if (body != null)
                {
                    normal = new Vector3(normal.x, 0f, normal.z).normalized;
                    thisRb.transform.root.GetComponent<BodyHandler>().Head.PartRigidbody.
                         AddForce(normal * _objectForceNormal * itemDamage, ForceMode.VelocityChange);
                    body.AddForce(Vector3.up * _objectForceUp, ForceMode.VelocityChange);
                    body.AddForce(normal * _objectForceNormal * itemDamage, ForceMode.VelocityChange);

                    Debug.Log("objcol  Hip");
                }
                else
                {
                    thisRb.AddForce(Vector3.up * _objectForceUp, ForceMode.VelocityChange);
                    thisRb.AddForce(normal * _objectForceNormal * itemDamage, ForceMode.VelocityChange);
                }
                break;
            case InteractableObject.Damage.Punch:
                thisRb.AddForce(normal * _punchForceNormal, ForceMode.VelocityChange);
                thisRb.AddForce(Vector3.up * _punchForceUp, ForceMode.VelocityChange);
                break;
            case InteractableObject.Damage.DropKick:
                if (body != null)
                {
                    thisRb.transform.root.GetComponent<BodyHandler>().Head.PartRigidbody.
                         AddForce(normal * _dropkickForceNormal, ForceMode.VelocityChange);
                    body.AddForce(Vector3.up * _dropkickForceUp, ForceMode.VelocityChange);
                    body.AddForce(normal * _dropkickForceNormal, ForceMode.VelocityChange);
                    Debug.Log("Dropcol  Hip");
                }
                else
                {
                    thisRb.AddForce(normal * _dropkickForceNormal, ForceMode.VelocityChange);
                    thisRb.AddForce(Vector3.up * _dropkickForceUp, ForceMode.VelocityChange);
                }
                break;
            case InteractableObject.Damage.Headbutt:
                thisRb.AddForce(normal * _headbuttForceNormal, ForceMode.VelocityChange);
                thisRb.AddForce(Vector3.up * _headbuttForceUp, ForceMode.VelocityChange);
                break;
            case InteractableObject.Damage.Knockout:
                thisRb.AddForce(normal * 10f, ForceMode.VelocityChange);
                break;
            default:
                break;
        }

        if (thisRb.velocity.magnitude > 15f)
        {
            thisRb.velocity = thisRb.velocity.normalized * 15f;
        }
        if (body != null && body.velocity.magnitude > 10f)
        {
            body.velocity = body.velocity.normalized * 10f;
            Debug.Log("maxVel");

        }
    }
}