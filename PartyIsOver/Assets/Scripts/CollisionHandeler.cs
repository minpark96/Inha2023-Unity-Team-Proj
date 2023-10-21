using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandeler : MonoBehaviour
{
    public float damageMinimumVelocity = 0.25f;

    public Actor actor;

    private Transform rootTransform;


    // Start is called before the first frame update
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

    // Update is called once per frame
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


            //데미지 적용
            num2 = Mathf.RoundToInt(num2);
            if (num2 > 0f && velocityMagnitude > damageMinimumVelocity)
            {
                if (collisionInteractable != null)
                {
                    actor.statusHandeler.AddDamage(collisionInteractable.damageModifier, num2, collisionCollider.gameObject);
                }
                else
                {
                    actor.statusHandeler.AddDamage(InteractableObject.Damage.Default, num2, collisionCollider.gameObject);
                }
            }
        }

}
