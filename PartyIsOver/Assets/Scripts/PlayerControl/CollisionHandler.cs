using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
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

    void Update()
    {

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
        float num2 = 0f;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);

            //충돌한 녀석이 rigidbody가 있으면 그 객체의 mass를 넣고 없으면 디폴트로 40을 넣음
            num = ((!collisionRigidbody) ? 40f : collisionRigidbody.mass);

            //충돌지점의 노멀벡터랑 relativeVelocity(충돌시 두 물체간의 상대적인 이동속도)를 곱하고 변환
            num2 = Vector3.Dot(contact.normal, relativeVelocity) * Mathf.Clamp(num, 0f, 40f) / 1100f;

            //음수면 양수로 바꿈
            if (num2 < 0f)
            {
                num2 = 0f - num2;
            }


            //충돌체 interactableObject의 데미지종류별로 케이스 나눔
            switch (collisionInteractable.damageModifier)
            {
                case InteractableObject.Damage.Ignore:
                    num2 = 0f;
                    break;
                case InteractableObject.Damage.Object:
                    num2 *= 20f;
                    //thisCollider.attachedRigidbody : 충돌발생시 두개의 충돌체중 충돌 정보를 얻기위해 사용된 콜라이더의 충돌체 
                    contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 5f, ForceMode.VelocityChange);
                    //actor.inputHandler.SetVibration(0.2f, 0f, 0.1f);
                    break;
                case InteractableObject.Damage.Punch:
                    {
                        Actor componentInParent = collisionInteractable.GetComponentInParent<Actor>();
                        contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 3f + Vector3.up * 2f, ForceMode.VelocityChange);
                        //actor.inputHandler.SetVibration(0.5f, 0f, 0.1f);
                        num2 *= 700f;
                        break;
                    }
                case InteractableObject.Damage.DivingKick:
                    //actor.applyedForce = 0.5f;
                    num2 *= 140f;
                    contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 25f, ForceMode.VelocityChange);
                    contact.thisCollider.attachedRigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
                    // actor.inputHandler.SetVibration(0.7f, 0f, 0.1f);
                    break;
                case InteractableObject.Damage.Headbutt:
                    //actor.applyedForce = 0.5f;
                    num2 *= 80f;
                    contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 20f, ForceMode.VelocityChange);
                    contact.thisCollider.attachedRigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
                    //actor.inputHandler.SetVibration(0.6f, 0f, 0.1f);
                    break;
                case InteractableObject.Damage.Knockout:
                    num2 = 1000000f;
                    contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 10f, ForceMode.VelocityChange);
                    //actor.inputHandler.SetVibration(1f, 0f, 0.2f);
                    break;
                case InteractableObject.Damage.Ice:
                    num2 = 1f;
                    break;
                case InteractableObject.Damage.ElectricShock:
                    num2 = 1f;
                    break;
                default:
                    contact.thisCollider.attachedRigidbody.AddForce(contact.normal * 10f, ForceMode.VelocityChange);
                    break;
                case InteractableObject.Damage.Default:
                    break;
            }


            //데미지 적용
            num2 = Mathf.RoundToInt(num2);


            //Debug.Log(velocityMagnitude);

            if (num2 > 0f && velocityMagnitude > damageMinimumVelocity)
            {
                if (collisionInteractable != null)
                {
                    actor.StatusHandler.AddDamage(collisionInteractable.damageModifier, num2, collisionCollider.gameObject);
                }
                else
                {
                    actor.StatusHandler.AddDamage(InteractableObject.Damage.Default, num2, collisionCollider.gameObject);
                }
            }



            //Debug.Log(collision.gameObject.name);

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            DamageCheck(collision);
    }
}