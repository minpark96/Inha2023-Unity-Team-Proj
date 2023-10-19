using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingHandeler : MonoBehaviour
{
    public float detectionRadius = 3f;
    public LayerMask layerMask;
    public float maxAngle = 90f; // 180도의 절반 (90도)으로 설정

    Collider _nearestCollider;


    InteractableObject[] _interactableObjects = new InteractableObject[30];
    InteractableObject _nearestObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Collider SearchTarget()
    {
        Collider[] colliders = new Collider[30];
        _nearestCollider = null;
        _nearestObject = null;

        // 캐릭터가 현재 바라보는 방향 벡터
        Vector3 detectionDirection = transform.forward;

        // 원 안에 콜라이더 검출
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, colliders, layerMask);


        if (colliderCount <= 0 )
        {
            return null;
        }



        // 바라보는 방향 180도 이내에 콜라이더 중 interatableObject 보유중인지 확인
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - transform.position;
            float angle = Vector3.Angle(detectionDirection, toCollider);

            if (angle <= maxAngle && colliders[i].GetComponentInChildren<InteractableObject>())
            {
                if (_nearestObject == null || toCollider.magnitude < (_nearestObject.transform.position - transform.position).magnitude)
                {

                    Debug.Log("Collider " + colliders[i].name);

                    _nearestCollider = colliders[i];


                }

            }
        }

        // 해당 콜라이더가 가진 interatableObject 들을 검출
        //_interactableObjects = _nearestCollider.GetComponentsInChildren<InteractableObject>();



        //가장 가까운 interatable 오브젝트 찾기 구현필요





        return _nearestCollider;
    }

}
