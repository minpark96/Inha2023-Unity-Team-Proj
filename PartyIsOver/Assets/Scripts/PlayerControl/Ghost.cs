using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;

public class Ghost : MonoBehaviourPunCallbacks
{
    public float MoveSpeed = 5.0f;
    public float TurnSpeed = 3.0f;
    public CameraControl CameraControl;
    
    Animator anim;
    Vector3 moveDir = new Vector3(0, 0, 0);
    Transform camPos;
    Vector3 prevPos;

    void Awake()
    {
        if (CameraControl == null)
        {
            Debug.Log("카메라 컨트롤 초기화");
            CameraControl = GetComponent<CameraControl>();
        }

        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;

        prevPos = transform.position;
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    camPos = CameraControl.CameraArm.transform;
                    if (Input.GetKey(KeyCode.W))
                    {
                        moveDir += camPos.forward;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        moveDir -= camPos.forward;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        moveDir -= camPos.right;
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        moveDir += camPos.right;
                    }

                    Turn(moveDir);
                    if (moveDir != Vector3.zero)
                    {
                        moveDir = moveDir.normalized;
                    }
                    break;
                }
        }
    }
    
    void OnDestroy()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
    }

    void Move()
    {
        transform.position += moveDir * MoveSpeed * Time.deltaTime;
        anim.SetBool("IsFly", moveDir != Vector3.zero);
        moveDir = Vector3.zero;
    }

    void Turn(Vector3 offset)
    {
        if (offset == Vector3.zero) return;
        Quaternion turnAngle = Quaternion.LookRotation(offset, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, turnAngle, TurnSpeed * Time.deltaTime);

        Vector3 turnDir = transform.position - prevPos;
        prevPos = transform.position;
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {

    }

    void Update()
    {
        CameraControl.LookAround(transform.position);
        CameraControl.CursorControl();
    }

    void FixedUpdate()
    {
        Move();
    }
}
