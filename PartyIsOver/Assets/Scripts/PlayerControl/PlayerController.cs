using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("앞뒤 속도")]
    public float Speed;
    [Header("좌우 속도")]
    public float StrafeSpeed;
    [Header("점프 힘")]
    public float JumpForce;

    public static int LayerCnt = 7;
    public string TestTag = "Item";

    private GameObject _hipGameObject;
    private Rigidbody _hipRigidbody;

    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

    public bool IsGrounded;

    private int _layerLog;

    void Start()
    {
        //if (photonView.IsMine == false) return;

        _hipGameObject = GameObject.Find("pelvis");
        _hipRigidbody = _hipGameObject.GetComponent<Rigidbody>();

        if(!photonView.IsMine)
            enabled = false;

        //실수를 방지하기 위해서 구독이 두번 들어오는걸 방지
        Managers.Input.KeyAction -= OnKeyboard;
        //어떤 키가 눌리면 구독신청 해버림
        Managers.Input.KeyAction += OnKeyboard;

        _layerLog = LayerCnt;
        ChangeLayerRecursively(gameObject, LayerCnt++);
        ChangeTagRecursively(gameObject, TestTag);

        Speed = 20.0f;
        StrafeSpeed = 20.0f;

        Debug.Log("Init LayerCnt: " + _layerLog);
        Debug.Log("IsMine?: " + photonView.IsMine);
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("SyncPosition",RpcTarget.Others,transform.position,transform.rotation);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, _networkRotation, Time.deltaTime * 10);
        }
    }

    [PunRPC]
    private void SyncPosition(Vector3 position, Quaternion rotation)
    {
        _networkPosition = position;
        _networkRotation = rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //보내기
            Debug.Log("보내기");
            stream.SendNext(_hipRigidbody.position);
            stream.SendNext(_hipRigidbody.rotation);
        }
        else
        {
            //받기
            Debug.Log("받기");
            _networkPosition = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public void FiexdUpdate()
    {
        if(!photonView.IsMine)
        {
            _hipRigidbody.position = Vector3.MoveTowards(_hipRigidbody.position, _networkPosition, Time.fixedDeltaTime);
            _hipRigidbody.rotation = Quaternion.RotateTowards(_hipRigidbody.rotation, _networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }

    private void OnKeyboard()
    {
        if (photonView.IsMine == false) return;

        Debug.Log("My LayerCnt: " + _layerLog);
        if (Input.GetKey(KeyCode.W))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(transform.forward * Speed * 2f);
            else
                _hipRigidbody.AddForce(transform.forward * Speed);

        if (Input.GetKey(KeyCode.S))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(-transform.forward * Speed * 2f);
            else
                _hipRigidbody.AddForce(-transform.forward * Speed);

        if (Input.GetKey(KeyCode.A))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(-transform.right * StrafeSpeed * 2f);
            else
                _hipRigidbody.AddForce(-transform.right * StrafeSpeed);

        if (Input.GetKey(KeyCode.D))
            if (Input.GetKey(KeyCode.LeftShift))
                _hipRigidbody.AddForce(transform.right * StrafeSpeed * 2f);
            else
                _hipRigidbody.AddForce(transform.right * StrafeSpeed);

        if (Input.GetAxis("Jump") > 0)
        {
            if (IsGrounded)
            {
                _hipRigidbody.AddForce(new Vector3(0, JumpForce, 0));
                IsGrounded = false;
            }
        }

    }

    private void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    }

    private void ChangeTagRecursively(GameObject obj, string tag)
    {
        obj.tag = tag;

        foreach (Transform child in obj.transform)
        {
            ChangeTagRecursively(child.gameObject, tag);
        }
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        //Debug.Log("Send LayerCnt: " + _layerLog);

    //        stream.SendNext(_hipRigidbody.position);
    //        stream.SendNext(_hipRigidbody.rotation);
    //        stream.SendNext(_hipRigidbody.velocity);
    //        stream.SendNext(_hipRigidbody.angularVelocity);

    //        //Debug.Log("Send position: " + _hipRigidbody.position);
    //        //Debug.Log("Send rotation: " + _hipRigidbody.rotation);
    //        //Debug.Log("Send velocity: " + _hipRigidbody.velocity);
    //        //Debug.Log("Send angularVelocity: " + _hipRigidbody.angularVelocity);

    //    }
    //    else if (stream.IsReading)
    //    {
    //        //Debug.Log("Receive LayerCnt: " + _layerLog);

    //        _hipRigidbody.position = (Vector3)stream.ReceiveNext();
    //        _hipRigidbody.rotation = (Quaternion)stream.ReceiveNext();
    //        _hipRigidbody.velocity = (Vector3)stream.ReceiveNext();
    //        _hipRigidbody.angularVelocity = (Vector3)stream.ReceiveNext();

    //        //Debug.Log("Receive position: " + _hipRigidbody.position);
    //        //Debug.Log("Receive rotation: " + _hipRigidbody.rotation);
    //        //Debug.Log("Receive velocity: " + _hipRigidbody.velocity);
    //        //Debug.Log("Receive angularVelocity: " + _hipRigidbody.angularVelocity);

    //    }
    //}
}


#region 창고
/*
 
포톤 뷰에서 싱크 한번 찾아보기
코드로 player1에서 서버로
서버에서 player1 player2로 서로 값을 보내주는 방법이 있다


------------------------------------------------------------------------------------------------------------------
코드로 추가 해볼려했는데 에러감
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_hipRigidbody.position);
            stream.SendNext(_hipRigidbody.rotation);
        }
        else
        {
            _networkPosition = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if(!photonView.IsMine)
        {
            _hipRigidbody.position = _networkPosition;
            _hipRigidbody.rotation = _networkRotation;
        }
    }

 
 */
#endregion