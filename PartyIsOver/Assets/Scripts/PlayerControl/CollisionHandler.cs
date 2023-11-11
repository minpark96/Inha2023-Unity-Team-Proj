using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviourPun
{
    public float damageMinimumVelocity = 0.25f;

    public Actor actor;

    private Transform rootTransform;

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
    }


    private void DamageCheck(Collision collision)
    {
        InteractableObject collisionInteractable = collision.transform.GetComponent<InteractableObject>();
        Transform collisionTransform = collision.transform;
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

                // 데미지 적용
                if (damage > 0f && velocityMagnitude > damageMinimumVelocity)
                {
                    if (collisionInteractable != null)
                    {
                        actor.StatusHandler.AddDamage(collisionInteractable.damageModifier, damage, collisionCollider.gameObject);
                    }
                    else
                    {
                        actor.StatusHandler.AddDamage(InteractableObject.Damage.Default, damage, collisionCollider.gameObject);
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
                else
                {
                    actor.StatusHandler.AddDamage(InteractableObject.Damage.Default, damage, collisionCollider.gameObject);
                }
            }
        }
    }

    private float ApplyBodyPartDamageModifier(float damage)
    {
        if (transform == actor.BodyHandler.RightArm.transform ||
            transform == actor.BodyHandler.LeftArm.transform)
            damage *= actor.ArmMultiple;
        else if (transform == actor.BodyHandler.RightForearm.transform ||
            transform == actor.BodyHandler.LeftForearm.transform)
            damage *= actor.ArmMultiple;
        else if (transform == actor.BodyHandler.RightHand.transform ||
            transform == actor.BodyHandler.LeftHand.transform)
            damage *= actor.HandMultiple;
        else if (transform == actor.BodyHandler.RightLeg.transform ||
            transform == actor.BodyHandler.LeftLeg.transform)
            damage *= actor.LegMultiple;
        else if (transform == actor.BodyHandler.RightThigh.transform ||
            transform == actor.BodyHandler.LeftThigh.transform)
            damage *= actor.LegMultiple;
        //else if (transform == actor.BodyHandler.RightFoot.transform ||
            //transform == actor.BodyHandler.LeftFoot.transform)
            //damage *= actor.LegMultiple;
        else if (transform == actor.BodyHandler.Head.transform)
            damage *= actor.HeadMultiple;

        return damage;
    }
    private float PhysicalDamage(InteractableObject collisionInteractable, float damage, ContactPoint contact)
    {
        switch (collisionInteractable.damageModifier)
        {
            case InteractableObject.Damage.Ignore:
                damage = 0f;
                break;
            case InteractableObject.Damage.Object:
                damage *= 20f;
                contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 5f, ForceMode.VelocityChange);
                break;
            case InteractableObject.Damage.Punch:
                damage *= 700f;
                contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 3f + Vector3.up * 2f, ForceMode.VelocityChange);
                break;
            case InteractableObject.Damage.DropKick:
                damage *= 1004f;
                contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 25f, ForceMode.VelocityChange);
                contact.thisCollider.attachedRigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
                break;
            case InteractableObject.Damage.Headbutt:
                damage *= 80f;
                contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 20f, ForceMode.VelocityChange);
                contact.thisCollider.attachedRigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
                break;
            case InteractableObject.Damage.Knockout:
                damage = 1000000f;
                contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 10f, ForceMode.VelocityChange);
                break;
            default:
                break;
        }

        return damage;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;
       
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            DamageCheck(collision);
    }
}