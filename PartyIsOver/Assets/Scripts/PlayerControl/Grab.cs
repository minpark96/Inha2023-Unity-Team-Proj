using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Grab : MonoBehaviourPun
{
    private TargetingHandler _targetingHandler;
    InteractableObject _searchTarget;

    bool _isRightGrab = false;
    bool _isLeftGrab = false;

    private GameObject _grabGameObject;
    private Rigidbody _leftHandRigid;
    private Rigidbody _rightHandRigid;

    private FixedJoint _grabNewJoint;

    // 양손 추가
    private FixedJoint _gameObjectJointLeft;
    private FixedJoint _gameObjectJointRight;

    // 양손 자세 추가
    private ConfigurableJoint _jointLeft;
    private ConfigurableJoint _jointRight;
    private ConfigurableJoint _jointLeftForeArm;
    private ConfigurableJoint _jointRightForeArm;
    private ConfigurableJoint _jointLeftUpperArm;
    private ConfigurableJoint _jointRightUpperArm;
    private ConfigurableJoint _jointChest;

    Vector3 targetPosition;


    private BodyHandler _bodyHandler;

    // 아이템 종류
    private int _itemType;
    public float _turnForce;

    public enum Side
    {
        Left = 0,
        Right = 1
    }

    void Start()
    {
        _bodyHandler = transform.root.GetComponent<BodyHandler>();
        _targetingHandler = GetComponent<TargetingHandler>();

        _bodyHandler.BodySetup();


        _leftHandRigid = _bodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = _bodyHandler.RightHand.PartRigidbody;

        _jointLeft = _bodyHandler.LeftHand.PartJoint;
        _jointRight = _bodyHandler.RightHand.PartJoint;

        _jointLeftForeArm = _bodyHandler.LeftForarm.PartJoint;
        _jointRightForeArm = _bodyHandler.RightForarm.PartJoint;

        _jointLeftUpperArm = _bodyHandler.LeftArm.PartJoint;
        _jointRightUpperArm = _bodyHandler.RightArm.PartJoint;

        _jointChest = _bodyHandler.Chest.PartJoint;


    }

    void Update()
    {
        if (_grabGameObject != null)
        {
            // 놓기
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("놨다");

                Destroy(_grabNewJoint);
                Destroy(_gameObjectJointLeft);
                Destroy(_gameObjectJointRight);

                _grabGameObject = null;
                _isRightGrab = false;
                _isLeftGrab = false;


                // 관절 복구
                _jointLeft.angularYMotion = ConfigurableJointMotion.Limited;
                _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointLeft.angularZMotion = ConfigurableJointMotion.Limited;
                _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Limited;
                _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Limited;

                _jointRight.angularYMotion = ConfigurableJointMotion.Limited;
                _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Limited;
                _jointRight.angularZMotion = ConfigurableJointMotion.Limited;
                _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Limited;
                _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Limited;
                return;
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
            _jointLeft.targetPosition = targetPosition + new Vector3(0, 0, 20);
            _jointRight.targetPosition = _jointLeft.targetPosition;

            _jointLeftForeArm.targetPosition = targetPosition;
            _jointRightForeArm.targetPosition = _jointLeftForeArm.targetPosition;
        }
    }


    public void Grabbing()
    {
        _searchTarget = _targetingHandler.SearchTarget();

        //발견한 오브젝트가 없으면 리턴
        if (_searchTarget == null)
            return;


  
        //서치타겟이 아이템이고, 0.5거리 이내에 있을때
        if (_searchTarget.GetComponent<Item>() != null && Vector3.Distance(_searchTarget.transform.position, _bodyHandler.Chest.transform.position) <= 1.5f)
        {
            Item item = _searchTarget.GetComponent<Item>();
            Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir * 80f);

            if(GrabCheck(Side.Right, item.OneHandedPos))
            {
                //잡기에 성공했을경우 관절 일부 고정
                _jointRight.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRight.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;

                //아이템이 양손형일경우 왼손도 그랩
                if (item.GetComponent<Item>().ItemType == Define.ItemType.TwoHanded ||
                 item.GetComponent<Item>().ItemType == Define.ItemType.Ranged)
                {

                }
            }
        }
        else
        {
            //서치타겟이 아이템이 아닐 때
            //타겟의 가장 가까운 지점으로 손을 뻗어서 접촉시 그랩상태로 들어감
            //타겟의 위치와 거리에 따라 양손그랩, 한손그랩이 들어감

        }
    }

    IEnumerator TwoHandedGrabbing()
    {


        yield return null;
    }

    bool GrabCheck(Side side, Transform target)
    {

        Transform hand;
        if (side == Side.Right)
        {
            hand = _rightHandRigid.transform;
            if (_isRightGrab)
                return false;
        }
        else
        {
            hand = _leftHandRigid.transform;
            if (_isLeftGrab)
                return false; 
        }


        //밑에 코드 양손 다 가능하게 바꿔야함
        if (Vector3.Distance(hand.position, target.position) <= 0.3f)
        {
            Debug.Log("Grab");

            _grabGameObject = target.root.gameObject;

            _grabNewJoint = _grabGameObject.AddComponent<FixedJoint>();
            _grabNewJoint.connectedBody = _rightHandRigid;
            _grabNewJoint.breakForce = 9001;

            _isRightGrab = true;
            return true;
        }

        return false;

    }

    private void OnTriggerStay(Collider other)
    {
        return;
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

                    _grabNewJoint = _grabGameObject.AddComponent<FixedJoint>();
                    _grabNewJoint.connectedBody = _leftHandRigid;
                    _grabNewJoint.breakForce = 9001;
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
                    _gameObjectJointLeft.connectedBody = _leftHandRigid;
                    _gameObjectJointLeft.breakForce = 9001;

                    _gameObjectJointRight = _grabGameObject.AddComponent<FixedJoint>();
                    _gameObjectJointRight.connectedBody = _rightHandRigid;
                    _gameObjectJointRight.breakForce = 9001;

                    // 양손 휘두르기 모션을 위해 관절 부분잠금
                    _jointLeft.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointLeft.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;

                    _jointRight.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                    _jointRight.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                    _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
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
        int forcingCount = 5000;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce*3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce*3, 0, 0));


        while (forcingCount > 0)
        {
            AlignToVector(_jointLeft.GetComponent<Rigidbody>(), _jointLeft.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointLeftForeArm.GetComponent<Rigidbody>(), _jointLeftForeArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);
            AlignToVector(_jointLeftUpperArm.GetComponent<Rigidbody>(), _jointLeftUpperArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 2f);

            AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, _jointLeft.transform.position, 0.1f, 2f);
            AlignToVector(_jointRightForeArm.GetComponent<Rigidbody>(), _jointRightForeArm.transform.position, _jointLeftForeArm.transform.position, 0.1f, 2f);
            AlignToVector(_jointRightUpperArm.GetComponent<Rigidbody>(), _jointRightUpperArm.transform.position, _jointLeftUpperArm.transform.position, 0.1f, 2f);

            AlignToVector(_jointChest.GetComponent<Rigidbody>(), _jointChest.transform.position, _jointLeft.transform.position, 0.1f, 2f);

            //AlignToVector(_jointRight.GetComponent<Rigidbody>(), _jointRight.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);
            //AlignToVector(_jointRightForeArm.GetComponent<Rigidbody>(), _jointRightForeArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);
            //AlignToVector(_jointRightUpperArm.GetComponent<Rigidbody>(), _jointRightUpperArm.transform.position, new Vector3(0.2f, 0f, 0f), 0.1f, 0.1f);

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
        }
    }
}
