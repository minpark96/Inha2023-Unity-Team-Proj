using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Define;

public class Grab : MonoBehaviourPun
{
    private TargetingHandler _targetingHandler;
    private InteractableObject _leftSearchTarget;
    private InteractableObject _rightSearchTarget;


    private Actor _actor;

    [SerializeField]
    bool _isRightGrab = false;
    [SerializeField]
    bool _isLeftGrab = false;


    [SerializeField]
    private float _throwingForce = 40f;

    float _grabDelayTimer = 0.5f;

    public bool _isGrabbingInProgress {get; private set;}


    public GameObject EquipItem;
    public GameObject LeftGrabObject;
    public GameObject RightGrabObject;

    public Transform RangeWeaponSkin;

    private Rigidbody _leftHandRigid;
    private Rigidbody _rightHandRigid;

    private FixedJoint _grabJointLeft;
    private FixedJoint _grabJointRight;

    private ConfigurableJoint _jointLeft;
    private ConfigurableJoint _jointRight;
    private ConfigurableJoint _jointLeftForeArm;
    private ConfigurableJoint _jointRightForeArm;
    private ConfigurableJoint _jointLeftUpperArm;
    private ConfigurableJoint _jointRightUpperArm;
    private ConfigurableJoint _jointChest;


    public GameObject CollisionObject;

    // 아이템 종류
    private int _itemType;
    public float _turnForce;

    public enum Side
    {
        Left = 0,
        Right = 1,
        Both = 2,
    }

    
    void Start()
    {
        _actor = GetComponent<Actor>();
        _actor.BodyHandler = transform.root.GetComponent<BodyHandler>();
        _targetingHandler = GetComponent<TargetingHandler>();
        _actor.BodyHandler.BodySetup();


        _leftHandRigid = _actor.BodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = _actor.BodyHandler.RightHand.PartRigidbody;

        _jointLeft = _actor.BodyHandler.LeftHand.PartJoint;
        _jointRight = _actor.BodyHandler.RightHand.PartJoint;

        _jointLeftForeArm = _actor.BodyHandler.LeftForearm.PartJoint;
        _jointRightForeArm = _actor.BodyHandler.RightForearm.PartJoint;

        _jointLeftUpperArm = _actor.BodyHandler.LeftArm.PartJoint;
        _jointRightUpperArm = _actor.BodyHandler.RightArm.PartJoint;

        _jointChest = _actor.BodyHandler.Chest.PartJoint;
    }

    void Update()
    {
        _grabDelayTimer -= Time.deltaTime;
        GrabStateCheck();

    }

    void GrabStateCheck()
    {
        PlayerLiftCheck();
        
        if(EquipItem != null)
        {
            _actor.GrabState = GrabState.EquipItem;
            return;
        }
        ClimbCheck();
    }


    void ClimbCheck()
    {
        if (_isRightGrab && _isLeftGrab && LeftGrabObject != null && RightGrabObject != null)
        {
            //나중에 아이템이나 플레이어가 아닌 오브젝트의 Layer를 ClimbLayer 등으로 통일하고 밑의 조건 바꿀 수 있음
            if(LeftGrabObject.GetComponent<CollisionHandler>() == null && RightGrabObject.GetComponent<CollisionHandler>() == null
                && LeftGrabObject.GetComponent<Item>() == null && RightGrabObject.GetComponent<Item>() == null)
            {
                _actor.GrabState = GrabState.Climb;
            }
        }
    }

    void PlayerLiftCheck()
    {
        if (_isRightGrab && _isLeftGrab && LeftGrabObject != null && RightGrabObject != null)
        {
            if (LeftGrabObject.GetComponent<CollisionHandler>() != null &&
                RightGrabObject.GetComponent<CollisionHandler>() != null)
            {
                _actor.GrabState = GrabState.PlayerLift;

                AlignToVector(_actor.BodyHandler.LeftArm.PartRigidbody, _actor.BodyHandler.LeftArm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward + _actor.BodyHandler.Chest.PartTransform.right / 2f + -_actor.PlayerController.MoveInput / 8f, 0.01f, 8f);
                AlignToVector(_actor.BodyHandler.LeftForearm.PartRigidbody, _actor.BodyHandler.LeftForearm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward, 0.01f, 8f);
                //_leftHandRigid.AddForce(Vector3.up*500);
                _leftHandRigid.AddForce(Vector3.up * 4, ForceMode.VelocityChange);

                //_actor.BodyHandler.Chest.PartRigidbody.AddForce(Vector3.down * 900);
                _actor.BodyHandler.Chest.PartRigidbody.AddForce(Vector3.down * 3, ForceMode.VelocityChange);

                AlignToVector(_actor.BodyHandler.RightArm.PartRigidbody, _actor.BodyHandler.RightArm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward + -_actor.BodyHandler.Chest.PartTransform.right / 2f + -_actor.PlayerController.MoveInput / 8f, 0.01f, 8f);
                AlignToVector(_actor.BodyHandler.RightForearm.PartRigidbody, _actor.BodyHandler.RightForearm.PartTransform.forward, -_actor.BodyHandler.Waist.PartTransform.forward, 0.01f, 8f);
                //_rightHandRigid.AddForce(Vector3.up*500);
                _rightHandRigid.AddForce(Vector3.up * 4, ForceMode.VelocityChange);
            }
        }
    }

    public void Climb()
    {
        GrabReset();
        //_rightHandRigid.AddForce(_rightHandRigid.transform.position + Vector3.down * 80f);
        //_leftHandRigid.AddForce(_rightHandRigid.transform.position + Vector3.down * 80f);

        
        _actor.BodyHandler.Hip.PartRigidbody.AddForce(Vector3.up * 100f, ForceMode.VelocityChange);
        _grabDelayTimer = 0.7f;

        Debug.Log("climbJump");
    }

    public void OnMouseEvent_EquipItem(Define.MouseEvent evt)
    {
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                }
                break;
            case Define.MouseEvent.Press:
                {

                }
                break;
            case Define.MouseEvent.PointerUp:
                {
                }
                break;
            case Define.MouseEvent.Click:
                {
                    ItemType type = EquipItem.GetComponent<Item>().ItemData.ItemType;

                    if (Input.GetMouseButtonUp(0))
                    {
                        switch(type)
                        {
                            case ItemType.OneHanded: StartCoroutine(OwnHandAttack());
                                break;
                            case ItemType.TwoHanded: StartCoroutine(HorizontalAttack());
                                break;
                            case ItemType.Gravestone: StartCoroutine(VerticalAttack());
                                break;
                            case ItemType.Ranged: UseItem();    
                                break;
                            case ItemType.Potion: StartCoroutine(UsePotionAnim());
                                break;
                        }
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        GrabReset();
                    }
                }
                break;
        }
    }

    public void OnMouseEvent_LiftPlayer(Define.MouseEvent evt)
    {
        switch (evt)
        {
            case Define.MouseEvent.PointerUp:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        GrabReset();
                    }
                }
                break;
            case Define.MouseEvent.PointerDown:
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        Rigidbody rb1 = RightGrabObject.GetComponent<Rigidbody>();
                        Rigidbody rb2 = LeftGrabObject.GetComponent<Rigidbody>();
                        GrabReset();

                        rb1.AddForce(-_actor.BodyHandler.Chest.PartTransform.up * _throwingForce,ForceMode.VelocityChange);
                        rb2.AddForce(-_actor.BodyHandler.Chest.PartTransform.up * _throwingForce,ForceMode.VelocityChange);

                        rb1.AddForce(Vector3.up * _throwingForce * 1.5f, ForceMode.VelocityChange);
                        rb2.AddForce(Vector3.up * _throwingForce * 1.5f, ForceMode.VelocityChange);
                    }
                }
                break;
        }
    }

    public void GrabPose()
    {
        if(EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.Ranged)
        {
            _jointLeft.targetPosition = EquipItem.GetComponent<Item>().TwoHandedPos.position;
            _jointRight.targetPosition = EquipItem.GetComponent<Item>().OneHandedPos.position;
        }
        else if(EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.OneHanded)
        {
            _jointRight.targetPosition = EquipItem.GetComponent<Item>().OneHandedPos.position;
        }
        else if(EquipItem.GetComponent<Item>().ItemData.ItemType == ItemType.Gravestone)
        {
            _jointLeft.targetPosition = EquipItem.GetComponent<Item>().TwoHandedPos.position;
            _jointRight.targetPosition = EquipItem.GetComponent<Item>().OneHandedPos.position;
        }

        // 기본 잡기 자세
        //targetPosition = _grabItem.transform.position;
        //_jointLeft.targetPosition = targetPosition + new Vector3(0, 0, 20);
        //_jointRight.targetPosition = _jointLeft.targetPosition;

        //_jointLeftForeArm.targetPosition = targetPosition;
        //_jointRightForeArm.targetPosition = _jointLeftForeArm.targetPosition;
    }

    public void GrabReset()
    {
        _isGrabbingInProgress = false;
        if(EquipItem != null)
        {
            EquipItem.gameObject.layer = LayerMask.NameToLayer("Item");
            EquipItem.GetComponent<Item>().Body.gameObject.SetActive(true);
            RangeWeaponSkin.gameObject.SetActive(false);
            EquipItem.GetComponent<Item>().Owner = null;
            EquipItem = null;

        }
        _grabDelayTimer = 0.5f;
        _isRightGrab = false;
        _isLeftGrab = false;
        RightGrabObject = null;
        LeftGrabObject = null;
        _actor.GrabState = GrabState.None;


        DestroyJoint();
    }

    public void Grabbing()
    {
        if (_grabDelayTimer > 0f)
            return;

        //타겟서치 태그설정 주의할것
        _leftSearchTarget = _targetingHandler.SearchTarget(Side.Left);
        _rightSearchTarget = _targetingHandler.SearchTarget(Side.Right);

        Debug.Log(_leftSearchTarget);
        Debug.Log(_rightSearchTarget);

        //발견한 오브젝트가 없으면 리턴
        if (_leftSearchTarget == null && _rightSearchTarget == null)
            return;

        _isGrabbingInProgress = true;

        //타겟이 정면에 있고 아이템일때
        if (_leftSearchTarget == _rightSearchTarget && _leftSearchTarget.GetComponent<Item>() != null)
        {
            //일정 거리 이내에 있을때 양손이 비어있을때
            if (Vector3.Distance(_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>()),
                _actor.BodyHandler.Chest.transform.position) <= 1.2f
                  && !_isRightGrab && !_isLeftGrab)
            {
                Item item = _leftSearchTarget.GetComponent<Item>();
                HandleItemGrabbing(item);
                return;
            }
        }
        else//아이템이 아닐때
        {
            Vector3 dir;

            //타겟이 정면이 아닐때
            if (_leftSearchTarget != null && !_isLeftGrab)
            {
                if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
                {
                    dir = ((_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>()) + Vector3.up)
                        - _leftHandRigid.transform.position).normalized;
                }
                else
                {
                    dir = (_targetingHandler.FindClosestCollisionPoint(_leftSearchTarget.GetComponent<Collider>())
                        - _leftHandRigid.transform.position).normalized;
                }

                _leftHandRigid.AddForce(dir * 80f);
                if(HandCollisionCheck(Side.Left))
                {
                    JointFix(Side.Left);
                    _grabDelayTimer = 0.5f;
                }
            }

            if(_rightSearchTarget != null && !_isRightGrab)
            {
                if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
                {
                    dir = ((_targetingHandler. FindClosestCollisionPoint(_rightSearchTarget.GetComponent<Collider>()) + Vector3.up)
                        - _rightHandRigid.transform.position).normalized;
                }
                else
                {
                    dir = (_targetingHandler.FindClosestCollisionPoint(_rightSearchTarget.GetComponent<Collider>())
                        - _rightHandRigid.transform.position).normalized;
                }

                _rightHandRigid.AddForce(dir * 80f);
                if (HandCollisionCheck(Side.Right))
                {
                    JointFix(Side.Right);
                    _grabDelayTimer = 0.5f;
                }
            }
        }
    }

 


    void HandleItemGrabbing(Item item)
    {
        switch (item.ItemData.ItemType)
        {
            case ItemType.OneHanded:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir.normalized * 80f);

                    if (IsHoldingItem(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //아이템에 맞게 관절조정 함수 추가해야함

                    JointFix(Side.Right);
                }
                break;
            case ItemType.TwoHanded:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Ranged:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Gravestone:
                {
                    TwoHandedGrab(item);
                }
                break;
            case ItemType.Potion:
                {
                    Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _rightHandRigid.AddForce(dir.normalized * 80f);

                    if (IsHoldingItem(item, Side.Right))
                        ItemRotate(item.transform, false);
                    else
                        return;
                    //아이템에 맞게 관절조정 함수 추가해야함

                    JointFix(Side.Right);
                }
                break;
        }
    }

    void TwoHandedGrab(Item item)
    {
        //아이템 방향따라 오른쪽 손잡이를 오른손으로 잡기 진행
        if (ItemDirCheck(item))
        {
            Vector3 dir = item.OneHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir.normalized * 90f);

            dir = item.TwoHandedPos.position - _leftHandRigid.transform.position;
            _leftHandRigid.AddForce(dir.normalized * 90f);

            if (IsHoldingItem(item, Side.Both))
                ItemRotate(item.transform, true);
            else
                return;
        }
        else
        {
            Vector3 dir = item.TwoHandedPos.position - _rightHandRigid.transform.position;
            _rightHandRigid.AddForce(dir.normalized * 90f);

            dir = item.OneHandedPos.position - _leftHandRigid.transform.position;
            _leftHandRigid.AddForce(dir.normalized * 90f);

            if (IsHoldingItem(item, Side.Both))
                ItemRotate(item.transform, false);
            else
                return;
        }

        JointFix(Side.Left);
        JointFix(Side.Right);
    }


    /// <summary>
    /// 손이 아이템에 제대로 접촉했는지 체크 후 관절생성
    /// </summary>
    bool IsHoldingItem(Item item,Side side)
    {
        //HandChecker 스크립트에서 양손 다 아이템의 손잡이와 접촉중인지 판정
        if (HandCollisionCheck(side))
        {
            _grabDelayTimer = 0.5f;
            EquipItem = item.transform.root.gameObject;
            EquipItem.GetComponent<Item>().Owner = GetComponent<Actor>();

            return true;
        }
        return false;
    }


    bool HandCollisionCheck(Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (_leftHandRigid.GetComponent<HandChecker>().CollisionObject != null &&
                    _leftHandRigid.GetComponent<HandChecker>().CollisionObject == _leftSearchTarget.gameObject)
                {
                    _isLeftGrab = true;
                    return true;
                }
                break;
            case Side.Right:
                if (_rightHandRigid.GetComponent<HandChecker>().CollisionObject != null && 
                    _rightHandRigid.GetComponent<HandChecker>().CollisionObject == _rightSearchTarget.gameObject)
                {
                    _isRightGrab = true;
                    return true;
                }
                break;
            case Side.Both:
                if (HandCollisionCheck(Side.Right) && HandCollisionCheck(Side.Left))   
                {
                    return true;
                }
                else
                {
                    _isRightGrab = false;
                    _isLeftGrab = false;
                }
                break;
        }

        return false;
    }

    bool ItemDirCheck(Item item)
    {
        //오른손과 손잡이 위치 체크해서 아이템 방향 리턴
        Vector3 toItem = (item.TwoHandedPos.position - _jointChest.transform.position).normalized; // 플레이어가 아이템을 바라보는 벡터
        Vector3 toOneHandedHandle = (item.OneHandedPos.position - _jointChest.transform.position).normalized; // 오른손이 잡아야할 oneHanded 손잡이 벡터
        Vector3 crossProduct = Vector3.Cross(toItem, toOneHandedHandle);

        if (crossProduct.y > 0) 
            return true;// 원핸드 손잡이가 오른쪽
        else
            return false;// 원핸드 손잡이가 왼쪽
    }


    /// <summary>
    /// 손 방향에 맞게 아이템 로테이션 조정
    /// </summary>
    void ItemRotate(Transform item, bool isHeadLeft)
    {
        //item.GetComponent<Rigidbody>().isKinematic = true;
        //item.GetComponent<Rigidbody>().useGravity = false;
        //item.GetComponent<Collider>().enabled = false;

        Vector3 targetPosition = _jointChest.transform.forward;

        switch (item.GetComponent<Item>().ItemData.ItemType)
        {
            case ItemType.TwoHanded:
                        //아이템의 헤드부분이 해당 방향벡터를 바라보게
                    if (isHeadLeft)
                        targetPosition = -_jointChest.transform.right;
                    else
                        targetPosition = _jointChest.transform.right;
                break;
            case ItemType.OneHanded:
                    targetPosition = _jointChest.transform.forward;
                break;
            case ItemType.Gravestone:
                {
                    if (isHeadLeft)
                        targetPosition = -_jointChest.transform.right;
                    else
                        targetPosition = _jointChest.transform.right;

                    item.transform.up = _jointChest.transform.up;
                }
                break;
            case ItemType.Ranged:
                {
                    item.GetComponent<Item>().Body.gameObject.SetActive(false);
                    RangeWeaponSkin.gameObject.SetActive(true);
                    targetPosition = -_jointChest.transform.up;
                }
                break;
            case ItemType.Potion:
                    targetPosition = _jointChest.transform.forward;
                break;
        }
        //item.gameObject.layer = gameObject.layer;
        item.transform.right = -targetPosition.normalized;

        GrabPose();
    }


    void JointFix(Side side)
    {
        ItemType type = ItemType.None;
        if (EquipItem != null)
        {
            type = EquipItem.GetComponent<Item>().ItemData.ItemType;
            EquipItem.gameObject.layer = gameObject.layer;
        }


        //잡기에 성공했을경우 관절 생성 및 일부 고정
        if (side == Side.Left)
        {
            _grabJointLeft = _leftSearchTarget.AddComponent<FixedJoint>();
            _grabJointLeft.connectedBody = _leftHandRigid;
            _grabJointLeft.breakForce = 9001;

            if (_leftSearchTarget != null)
                LeftGrabObject = _leftSearchTarget.gameObject;

            if (EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            {
                _jointLeft.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeftForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeftUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointLeft.angularZMotion = ConfigurableJointMotion.Locked;
                _jointLeftForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointLeftUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
            }

        }
        else if (side == Side.Right)
        {
            _grabJointRight = _rightSearchTarget.AddComponent<FixedJoint>();
            _grabJointRight.connectedBody = _rightHandRigid;
            _grabJointRight.breakForce = 9001;

            if (_rightSearchTarget != null)
                RightGrabObject = _rightSearchTarget.gameObject;

            if (EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            {
                _jointRight.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularYMotion = ConfigurableJointMotion.Locked;
                _jointRight.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightForeArm.angularZMotion = ConfigurableJointMotion.Locked;
                _jointRightUpperArm.angularZMotion = ConfigurableJointMotion.Locked;
            }
        }
    }


    
    void DestroyJoint()
    {
        Destroy(_grabJointLeft);
        Destroy(_grabJointRight);

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
    }


    IEnumerator VerticalAttack()
    {
        yield return _actor.PlayerController.DropRip(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.5f, 0.1f);
    }

    IEnumerator OwnHandAttack()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(0, _turnForce, 0));
        yield return _actor.PlayerController.ItemOwnHand(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.5f, 0.1f);
    }

    IEnumerator HorizontalAttack()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return _actor.PlayerController.ItemTwoHand(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.1f, 3f);
    }

    IEnumerator UsePotionAnim()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return _actor.PlayerController.Potion(PlayerController.Side.Right, 0.07f, 0.1f, 0.5f, 0.5f, 0.1f);

    }

    private void UseItem()
    {
        EquipItem.GetComponent<Item>().Use();
    }


    //리지드바디 part의 alignmentVector방향을 targetVector방향으로 회전
    private void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
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
