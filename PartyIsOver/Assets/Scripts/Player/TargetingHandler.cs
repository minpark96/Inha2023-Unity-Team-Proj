using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//�÷��̾ �ٸ� ������Ʈ�� �����ϰ�, �����ϴ� ����� ��Ƶ� Ŭ����

public static class TargetingHandler
{
    [SerializeField]
    static float _defaultDetectionRadius = 1f;
    [SerializeField]
    static float _inAirDetectionRadius = 2f;
    [SerializeField]
    static float maxAngle = 110f; // ���鿡�� �¿�� �ش� ������ŭ ��ġ

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

        //���߿� ��������� ���� ȿ����
        Collider[] colliders = new Collider[40];
        _nearestCollider = null;
        _nearestObject = null;
        _nearestDistance = Mathf.Infinity;
        _colliderCount = 0;
        _detectionRadius = _defaultDetectionRadius;

        //���麤��
        Vector3 chestForward = -chestTransform.up;

        //üũ�� ���� ����
        Vector3 detectionDirection;
        detectionDirection = (side == Define.Side.Left) ? -chestTransform.right : chestTransform.right;


        if (!isGrounded)
            _detectionRadius = _inAirDetectionRadius;

        //colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + chestForward, detectionRadius, colliders, layerMask);
        _colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + Vector3.up * 0.1f, _detectionRadius, colliders, layerMask);

        if (_colliderCount <= 0 )
            return null;

        // �ٶ󺸴� ���� 180�� �̳��� �ݶ��̴� �� interatableObject ���������� Ȯ��
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

                //��ġŸ���� ���׵��ϰ�� �߿䵵�� ���� �� ������ ���� ���ؼ� ����Ÿ���� �� ���ɼ��� ����
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

                //���尡��� Ÿ�� ����
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

        //Debug.Log(_nearestObject.gameObject + "�ֿ켱����");
        return _nearestObject;
    }

    //Ÿ�ٿ��� �տ� ���� ����� �浹������ ã�� �Լ�
    public static Vector3 FindClosestCollisionPoint(Vector3 start, Collider targetCol, int layer)
    {
        if (targetCol == null)
        {
            Debug.Log("Ÿ�ٿ� �ݶ��̴��� ����");
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
            Debug.Log("Ÿ�ٿ� ������ ����");
            return Vector3.zero;
        }
    }

    public static float TargetDistance(Vector3 target)
    {
        return Vector3.Distance(target,chestTransform.position);
    }
}
