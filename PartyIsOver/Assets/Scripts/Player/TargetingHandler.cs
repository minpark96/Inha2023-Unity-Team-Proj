using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//플레이어가 다른 오브젝트를 감지하고, 판정하는 기능을 모아둔 클래스

public static class TargetingHandler
{
    [SerializeField]
    static float _defaultDetectionRadius = 1f;
    [SerializeField]
    static float _inAirDetectionRadius = 2f;
    [SerializeField]
    static float maxAngle = 110f; // 정면에서 좌우로 해당 각도만큼 서치

    static float _detectionRadius;
    static float _nearestDistance;
    static int _colliderCount;


    static Collider _nearestCollider;
    static InteractableObject _nearestObject;
    static Transform chestTransform;
    static LayerMask defalultLayerMask;
    private static bool isInitialized = false;

    public static void Init()
    {
        isInitialized = true;
        defalultLayerMask |= 1 << (int)Define.Layer.Item;
        defalultLayerMask |= 1 << (int)Define.Layer.ClimbObject;
        defalultLayerMask |= 1 << (int)Define.Layer.InteractableObject;
    }
    public static LayerMask LayerSetting(int layer)
    {
        if (!isInitialized)
            Init();

        LayerMask layerMask = defalultLayerMask;

        for (int i = 0; i < 6; i++)
        {
            if (layer != (int)Define.Layer.Player1 + i)
                layerMask |= 1 << (int)Define.Layer.Player1 + i;
        }

        return layerMask;
    }


    public static InteractableObject SearchTarget(Define.Side side,Transform transform, int layer, bool isGrounded)
    {
        LayerMask layerMask = LayerSetting(layer);
        chestTransform = transform;

        //나중에 멤버변수로 빼야 효율적
        Collider[] colliders = new Collider[40];
        _nearestCollider = null;
        _nearestObject = null;
        _nearestDistance = Mathf.Infinity;
        _colliderCount = 0;
        _detectionRadius = _defaultDetectionRadius;

        //정면벡터
        Vector3 chestForward = -chestTransform.up;

        //체크할 방향 벡터
        Vector3 detectionDirection;
        detectionDirection = (side == Define.Side.Left) ? -chestTransform.right : chestTransform.right;


        if (!isGrounded)
            _detectionRadius = _inAirDetectionRadius;

        //colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + chestForward, detectionRadius, colliders, layerMask);
        _colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + Vector3.up * 0.1f, _detectionRadius, colliders, layerMask);

        if (_colliderCount <= 0 )
            return null;

        // 바라보는 방향 180도 이내에 콜라이더 중 interatableObject 보유중인지 확인
        for (int i = 0; i < _colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - chestTransform.position;
            float angle = Vector3.Angle(chestForward, toCollider);
            float angle2 = Vector3.Angle(detectionDirection, toCollider);

            if (angle <= maxAngle && angle2 <= 150f && colliders[i].GetComponent<InteractableObject>())
            {

                float distanceWithPriority = Vector3.Distance
                    (FindClosestCollisionPoint(chestTransform.position,colliders[i],layer),chestTransform.position);
                bool lowPriorityPart = true;

                //서치타겟이 래그돌일경우 중요도가 낮은 몸 부위에 값을 곱해서 최종타겟이 될 가능성을 낮춤
                if (colliders[i].GetComponent<BodyPart>() !=null)
                {
                    for (int j = (int)Define.BodyPart.Hip; j < (int)Define.BodyPart.Hip + 1; j++)
                    {
                        if (colliders[i].gameObject == colliders[i].transform.root.GetComponent<BodyHandler>().BodyParts[j].gameObject)
                            lowPriorityPart = false;
                    }

                    if (lowPriorityPart)
                        distanceWithPriority *= 10f;
                }

                //가장가까운 타겟 갱신
                if (_nearestObject == null || distanceWithPriority < _nearestDistance)
                {
                    _nearestCollider = colliders[i];
                    _nearestObject = colliders[i].GetComponent<InteractableObject>();
                    _nearestDistance = Vector3.Distance
                        (FindClosestCollisionPoint(chestTransform.position,_nearestCollider,layer), chestTransform.position);
                }
            }
        }


        if (_nearestCollider == null)
            return null;

        //Debug.Log(_nearestObject.gameObject + "최우선순위");
        return _nearestObject;
    }

    //타겟에서 손에 가장 가까운 충돌지점을 찾는 함수
    public static Vector3 FindClosestCollisionPoint(Vector3 start, Collider targetCol, int layer)
    {
        if (targetCol == null)
        {
            Debug.Log("타겟에 콜라이더가 없음");
            return Vector3.zero;
        }

        Vector3 startPos = start;
        Vector3 direction = (targetCol.transform.position - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetCol.transform.position);

        RaycastHit hit;
        LayerMask layerMask = LayerSetting(layer);

        if (Physics.Raycast(startPos, direction, out hit, distance, layerMask))
            return hit.point;
        else
        {
            Debug.Log("타겟에 문제가 있음");
            return Vector3.zero;
        }
    }

    public static float TargetDistance(Vector3 target)
    {
        return Vector3.Distance(target,chestTransform.position);
    }
}
