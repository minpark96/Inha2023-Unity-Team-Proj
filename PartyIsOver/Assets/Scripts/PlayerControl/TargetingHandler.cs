using UnityEngine;

public class TargetingHandler : MonoBehaviour
{
    public float detectionRadius = 1.5f;
    public LayerMask layerMask;
    public float maxAngle = 90f; // 180도의 절반 (90도)으로 설정

    Collider _nearestCollider;

    BodyHandler _bodyHandler;
    InteractableObject[] _interactableObjects = new InteractableObject[30];
    InteractableObject _nearestObject;

    // Start is called before the first frame update
    void Start()
    {
        _bodyHandler = GetComponent<BodyHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public InteractableObject SearchTarget()
    {
        //나중에 멤버변수로 빼야 효율적
        Collider[] colliders = new Collider[40];
        _nearestCollider = null;
        _nearestObject = null;

        Transform chestTransform = _bodyHandler.Chest.transform;
        Vector3 chestForward = -chestTransform.up;

        // 캐릭터가 현재 바라보는 방향 벡터
        Vector3 detectionDirection = chestForward;

        // 원 안에 콜라이더 검출
        int colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position, detectionRadius, colliders, layerMask);


        if (colliderCount <= 0 )
        {
            return null;
        }



        // 바라보는 방향 180도 이내에 콜라이더 중 interatableObject 보유중인지 확인
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - chestTransform.position;
            float angle = Vector3.Angle(detectionDirection, toCollider);

            if (angle <= maxAngle && colliders[i].GetComponent<InteractableObject>())
            {
                if (_nearestObject == null || toCollider.magnitude < (_nearestObject.transform.position - chestTransform.position).magnitude)
                {
                    _nearestCollider = colliders[i];
                    _nearestObject = colliders[i].GetComponent<InteractableObject>();
                }
            }
        }

        
        if(_nearestCollider == null)
        {
            return null;
        }


        return _nearestObject;
    }


    public Vector3 FindClosestCollisionPoint()
    {
        if (_nearestObject == null || _nearestCollider == null)
        {
            return Vector3.zero;
        }

        Vector3 start = _bodyHandler.Chest.transform.position;
        Vector3 direction = (_nearestCollider.transform.position - start).normalized;
        float distance = Vector3.Distance(start, _nearestCollider.transform.position);

        RaycastHit hit;

        if (Physics.Raycast(start, direction, out hit, distance, layerMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
