using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TargetingHandler : MonoBehaviour
{
    private float _detectionRadius = 1f;
    public LayerMask layerMask;
    private float maxAngle = 110f; // 정면에서 좌우로 해당 각도만큼 서치

    Collider _nearestCollider;
    private float _nearestDistance;

    Actor _actor;
    InteractableObject[] _interactableObjects = new InteractableObject[30];
    InteractableObject _nearestObject;

    // Start is called before the first frame update
    void Start()
    {
        _actor = GetComponent<Actor>();
        layerMask |= 1 << (int)Define.Layer.Item;
        layerMask |= 1 << (int)Define.Layer.ClimbObject;
        layerMask |= 1 << (int)Define.Layer.InteractableObject;

        for (int i = 0; i < 6; i++)
        {
            if (gameObject.layer != (int)Define.Layer.Player1 + i)
                layerMask |= 1 << (int)Define.Layer.Player1 + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 

    public InteractableObject SearchTarget(Grab.Side side)
    {
        //나중에 멤버변수로 빼야 효율적
        Collider[] colliders = new Collider[40];
        _nearestCollider = null;
        _nearestObject = null;
        _nearestDistance = Mathf.Infinity;
        Transform chestTransform = _actor.BodyHandler.Chest.transform;
        
        
        //정면벡터
        Vector3 chestForward = -chestTransform.up;

        //체크할 방향 벡터
        Vector3 detectionDirection;
        if (side == Grab.Side.Left)
            detectionDirection = -chestTransform.right;
        else
            detectionDirection = chestTransform.right;

        float detectionRadius = _detectionRadius;

        int colliderCount;

        if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
        {
            detectionRadius += 1f;
            //colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + chestForward, detectionRadius, colliders, layerMask);
            colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + Vector3.up * 0.1f, detectionRadius, colliders, layerMask);

        }
        else
        {
            // 원 안에 콜라이더 검출
            colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + Vector3.up * 0.1f, detectionRadius, colliders, layerMask);
        }

        if (colliderCount <= 0 )
        {
            return null;
        }

        // 바라보는 방향 180도 이내에 콜라이더 중 interatableObject 보유중인지 확인
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - chestTransform.position;
            float angle = Vector3.Angle(chestForward, toCollider);
            float angle2 = Vector3.Angle(detectionDirection, toCollider);

            if (angle <= maxAngle && angle2 <= 150f && colliders[i].GetComponent<InteractableObject>())
            {

                float distanceWithPriority = Vector3.Distance(FindClosestCollisionPoint(colliders[i]),chestTransform.position);
                bool lowPriorityPart = true;

                //서치타겟이 래그돌일경우 중요도가 낮은 몸 부위에 값을 곱해서 최종타겟이 될 가능성을 낮춤
                if (colliders[i].GetComponent<BodyPart>() !=null)
                {
                    for (int j = (int)Define.BodyPart.Hip; j < (int)Define.BodyPart.Hip + 1; j++)
                    {
                        if (colliders[i].gameObject == colliders[i].transform.root.GetComponent<BodyHandler>().BodyParts[j].gameObject)
                        {
                            lowPriorityPart = false;
                        }
                    }

                    if (lowPriorityPart)
                    {
                        distanceWithPriority *= 10f;
                    }
                }

                //가장가까운 타겟 갱신
                if (_nearestObject == null || distanceWithPriority < _nearestDistance)
                {
                    _nearestCollider = colliders[i];
                    _nearestObject = colliders[i].GetComponent<InteractableObject>();
                    _nearestDistance = Vector3.Distance(FindClosestCollisionPoint(_nearestCollider), chestTransform.position);
                }
            }
        }

        
        if(_nearestCollider == null)
        {
            return null;
        }

        //Debug.Log(_nearestObject.gameObject + "최우선순위");
        return _nearestObject;
    }

    public Vector3 FindClosestCollisionPoint(Collider collider)
    {
        if (collider == null)
        {
            Debug.Log("타겟에 콜라이더가 없음");
            return Vector3.zero;
        }

        Vector3 start = _actor.BodyHandler.Chest.transform.position; 
        Vector3 direction = (collider.transform.position - start).normalized;
        float distance = Vector3.Distance(start, collider.transform.position);

        RaycastHit hit;

        if (Physics.Raycast(start, direction, out hit, distance, layerMask))
        {
            return hit.point;
        }
        else
        {
            Debug.Log("타겟에 문제가 있음");
            return Vector3.zero;
        }
    }
}
