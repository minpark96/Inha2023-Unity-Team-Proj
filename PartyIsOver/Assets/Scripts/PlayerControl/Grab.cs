using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviourPun
{
    private TargetingHandler targetingHandler;


    private GameObject _grabGameObject;
    private Rigidbody _grabRigidbody;

    private FixedJoint _gameObjectJoint;

    // 양손 추가
    private FixedJoint _gameObjectJointLeft;
    private FixedJoint _gameObjectJointRight;
    private Rigidbody _grabRigidbody2;

    // 양손 자세 추가
    private ConfigurableJoint _jointLeft;
    private ConfigurableJoint _jointRight;
    private ConfigurableJoint _jointLeftForeArm;
    private ConfigurableJoint _jointRightForeArm;
    Vector3 targetPosition;

    // 아이템 종류
    private int _itemType;
    public float _turnForce;

    void Start()
    {
        _grabRigidbody = GetComponent<Rigidbody>();
        _grabRigidbody2 = GameObject.Find("GreenFistR").GetComponent<Rigidbody>();

        _jointLeft = GetComponent<ConfigurableJoint>();
        _jointRight = GameObject.Find("GreenFistR").GetComponent<ConfigurableJoint>();

        _jointLeftForeArm = GameObject.Find("GreenForeArmL").GetComponent<ConfigurableJoint>();
        _jointRightForeArm = GameObject.Find("GreenForeArmR").GetComponent<ConfigurableJoint>();

        targetingHandler = GetComponent<TargetingHandler>();
    }

    void Update()
    {
        if (_grabGameObject != null)
        {
            // 놓기
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("놨다");

                Destroy(_gameObjectJoint);
                Destroy(_gameObjectJointLeft);
                Destroy(_gameObjectJointRight);

                _grabGameObject = null;
            }

            // 임시 코드
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
            targetPosition = _grabGameObject.transform.position;
            _jointLeft.targetPosition = targetPosition + new Vector3(10, -10, 0);
            _jointRight.targetPosition = _jointLeft.targetPosition;

            _jointLeftForeArm.targetPosition = targetPosition;
            _jointRightForeArm.targetPosition = _jointLeftForeArm.targetPosition;

            _jointLeftForeArm.GetComponent<Rigidbody>().AddForce(new Vector3(0, 3f, 0));
            _jointRightForeArm.GetComponent<Rigidbody>().AddForce(new Vector3(3f, 3f, 0));
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
            //마우스를 클릭한 순간 앞에 오브젝트를 탐색하고
            //targetingHandler.SearchTarget();

            // 한손
            if (Input.GetKey(KeyCode.F))
            {
                if (_grabGameObject == null)
                {
                    Debug.Log("한손 잡았다");

                    _grabGameObject = other.gameObject;

                    _gameObjectJoint = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJoint.connectedBody = _grabRigidbody;
                    _gameObjectJoint.breakForce = 9001;
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
                    _gameObjectJointLeft.connectedBody = _grabRigidbody;
                    _gameObjectJointLeft.breakForce = 9001;

                    _gameObjectJointRight = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointRight.connectedBody = _grabRigidbody2;
                    _gameObjectJointRight.breakForce = 9001;
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
        StartCoroutine("Item2Action", grabGameObj);
    }
    // 접이식 의자: 찌르고 위에서 아래로 휘두르기
    private void Item3(GameObject grabGameObj)
    {
        StartCoroutine("Item3Action", grabGameObj);
    }

    IEnumerator Item1Action(GameObject grabGameObj)
    {
        int forcingCount = 2000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));

        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(0f, 0.2f, 0f), 0.1f, 6f);
            forcingCount--;
        }
        Debug.Log("코루틴 끝");

        yield return 0;
    }
    IEnumerator Item2Action(GameObject grabGameObj)
    {
        int forcingCount = 10000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 2.5f, 0, 0));
        //_jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 5, 0, 0));
        _jointLeftForeArm.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 2.5f, 0, 0));
        //_jointRightArm.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 50, 0, 0));

        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointLeftArm.GetComponent<Rigidbody>(), _jointLeftArm.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointRightArm.GetComponent<Rigidbody>(), _jointRightArm.transform.position, new Vector3(1f, 0f, 0f), 0.1f, 2f);
            forcingCount--;
        }
        Debug.Log("코루틴 끝");

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
            {
                Debug.DrawRay(part.position, alignmentVector * 0.2f, Color.red, 0f, depthTest: false);
                Debug.DrawRay(part.position, targetVector * 0.2f, Color.green, 0f, depthTest: false);
            }
        }
    }
}
