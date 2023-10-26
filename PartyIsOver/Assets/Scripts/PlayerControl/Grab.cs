using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviourPun
{
    // 잡기시 Item에 붙여지는 component
    private GameObject _grabGameObject;
    private FixedJoint _gameObjectJoint; // 한손일때
    private FixedJoint _gameObjectJointLeft; // 양손일때
    private FixedJoint _gameObjectJointRight;
    Vector3 targetPosition; // 아이템 위치

    // 아이템 종류
    private int _itemType;

    [SerializeField]
    private BodyHandler bodyHandler;

    [Header("잡기 모션 힘")]
    [SerializeField]
    private float _turnForce; 



    void Init()
    {
        bodyHandler = GetComponent<BodyHandler>();
    }

    void Update()
    {
        if (_grabGameObject != null)
        {
            targetPosition = _grabGameObject.transform.position;

            // 놓기
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("놨다");

                Destroy(_gameObjectJoint);
                Destroy(_gameObjectJointLeft);
                Destroy(_gameObjectJointRight);

                _grabGameObject = null;

                // 관절 복구
                bodyHandler.LeftHand.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.LeftForarm.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.LeftArm.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.LeftHand.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;
                bodyHandler.LeftForarm.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;
                bodyHandler.LeftArm.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;

                bodyHandler.RightHand.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.RightForarm.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.RightArm.PartJoint.angularYMotion = ConfigurableJointMotion.Limited;
                bodyHandler.RightHand.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;
                bodyHandler.RightForarm.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;
                bodyHandler.RightArm.PartJoint.angularZMotion = ConfigurableJointMotion.Limited;
                return;
            }
            
            // 아이템 종류 변경 임시 코드
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("아이템 타입1");
                _itemType = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("아이템 타입2");
                _itemType = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("아이템 타입3");
                _itemType = 3;
            }

            // 펀치와 합칠 필요 있음 > Input.GetMouseButtonDown(0)
            if (Input.GetKeyDown(KeyCode.L))
            {
                switch (_itemType)
                {
                    case 1:
                        Item1(_grabGameObject);
                        break;
                    case 2:
                        Item2(_grabGameObject);
                        break;
                    case 3:
                        Item3(_grabGameObject);
                        break;
                }
            }


            // 기본 잡기 자세
            bodyHandler.LeftHand.PartJoint.targetPosition = targetPosition + new Vector3(0, 0, 20);
            bodyHandler.RightHand.PartJoint.targetPosition = bodyHandler.LeftHand.PartJoint.targetPosition;
            bodyHandler.LeftForarm.PartJoint.targetPosition = targetPosition;
            bodyHandler.RightForarm.PartJoint.targetPosition = bodyHandler.LeftForarm.PartJoint.targetPosition;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //// 적 잡기
        //if (other.gameObject.layer == LayerMask.NameToLayer("Player2"))
        //{
        //    Debug.Log("player2 grab");
        //    if (Input.GetMouseButton(0))
        //    {
        //        if (_grabGameObject == null)
        //        {
        //            _grabGameObject = other.gameObject;

        //            _gameObjectJoint = _grabGameObject.AddComponent<FixedJoint>();
        //            _gameObjectJoint.connectedBody = _grabRigidbody;
        //            _gameObjectJoint.breakForce = 9001;
        //        }
        //    }
        //    else if (_grabGameObject != null)
        //    {
        //        Destroy(_grabGameObject.GetComponent<FixedJoint>());
        //        _gameObjectJoint = null;
        //        _grabGameObject = null;
        //    }
        //}

        
        // 아이템 잡기
        if (other.gameObject.CompareTag("Item"))
        {
            //마우스를 클릭한 순간 앞에 오브젝트를 탐색하고 << 추가 필요
            //targetingHandler.SearchTarget();


            // 한손
            if (Input.GetKey(KeyCode.F))
            {
                if (_grabGameObject == null)
                {
                    Debug.Log("한손 잡았다");

                    _grabGameObject = other.gameObject;

                    _gameObjectJoint = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJoint.connectedBody = bodyHandler.LeftHand.PartRigidbody;
                    _gameObjectJoint.breakForce = 9001;

                    // 한손 휘두르기 모션을 위해 관절 부분잠금
                    bodyHandler.LeftHand.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftForarm.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftArm.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftHand.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftForarm.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftArm.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                }
            }
            // 양손
            else if (Input.GetKey(KeyCode.G))
            {
                if (_grabGameObject == null)
                {
                    Debug.Log("양손 잡았다");

                    _grabGameObject = other.gameObject;

                    _gameObjectJointLeft = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointLeft.connectedBody = bodyHandler.LeftHand.PartRigidbody;
                    _gameObjectJointLeft.breakForce = 9001;

                    _gameObjectJointRight = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointRight.connectedBody = bodyHandler.RightHand.PartRigidbody;
                    _gameObjectJointRight.breakForce = 9001;

                    // 양손 휘두르기 모션을 위해 관절 부분잠금
                    bodyHandler.LeftHand.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftForarm.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftArm.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftHand.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftForarm.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.LeftArm.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;

                    bodyHandler.RightHand.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.RightForarm.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.RightArm.PartJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.RightHand.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.RightForarm.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                    bodyHandler.RightArm.PartJoint.angularZMotion = ConfigurableJointMotion.Locked;
                }
            }
        }
    }

    // 묘비: 위에서 아래로 찍어내리기
    private void Item1(GameObject grabGameObj)
    {
        StartCoroutine("Item1Action", grabGameObj);
    }
    // 냉동참치: 360도 회전
    private void Item2(GameObject grabGameObj)
    {
        // 잡고 좌클릭(펀치)으로 대체
        // void ArmActionPunching() 함수에서 수치 변경
        // rigidbody.AddForce(-(zero * 8f), ForceMode.VelocityChange); // 수정 before 2 after 8
        // rigidbody2.AddForce(zero * 12f, ForceMode.VelocityChange);  // 수정 before 3 after 12
    }
    // 접이식 의자: 찌르고 위에서 아래로 휘두르기
    private void Item3(GameObject grabGameObj)
    {
        StartCoroutine("Item3Action", grabGameObj);
    }

    IEnumerator Item1Action(GameObject grabGameObj)
    {
        int forcingCount = 2000;

        bodyHandler.LeftHand.PartRigidbody.AddForce(new Vector3(0, _turnForce, 0));
        bodyHandler.RightHand.PartRigidbody.AddForce(new Vector3(0, _turnForce, 0));

        while (forcingCount > 0)
        {
            AlignToVector(bodyHandler.LeftHand.PartRigidbody, bodyHandler.LeftHand.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            AlignToVector(bodyHandler.RightHand.PartRigidbody, bodyHandler.RightHand.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            forcingCount--;
        }
        Debug.Log("아래위로 휘두르기");

        yield return 0;
    }

    IEnumerator Item3Action(GameObject grabGameObj)
    {
        int forcingCount = 2000;

        bodyHandler.LeftHand.PartRigidbody.AddForce(new Vector3(0, 0, _turnForce*2));
        bodyHandler.RightHand.PartRigidbody.AddForce(new Vector3(0, 0, _turnForce*2));

        while (forcingCount > 0)
        {
            AlignToVector(bodyHandler.LeftHand.PartRigidbody, bodyHandler.LeftHand.transform.position, new Vector3(0f, 0f, 0.2f), 0.1f, 6f);
            AlignToVector(bodyHandler.RightHand.PartRigidbody, bodyHandler.RightHand.transform.position, new Vector3(0f, 0f, 0.2f), 0.1f, 6f);
            forcingCount--;
        }
        Debug.Log("찌르기");


        yield return new WaitForSeconds(0.5f);

        StartCoroutine("Item1Action", grabGameObj);

        yield return 0;
    }


    //리지드바디 part의 alignmentVector방향을 targetVector방향으로 회전
    public void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
    {
        if (part == null)
        {
            return;
        }
        Vector3 vector = Vector3.Cross(Quaternion.AngleAxis(part.angularVelocity.magnitude * 57.29578f * stability / speed, part.angularVelocity) * alignmentVector, targetVector * 10f);
        if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z))
        {
            part.AddTorque(vector * speed * speed);
        }
    }
}
