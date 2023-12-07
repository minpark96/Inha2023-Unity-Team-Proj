using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;

public class Ghost : MonoBehaviourPunCallbacks
{
    public float Speed = 5.0f;

    public CameraControl CameraControl;
    Animator anim;
    Vector3 moveDir = new Vector3(0, 0, 0);

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
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        moveDir += Vector3.forward;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        moveDir -= Vector3.forward;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        moveDir -= Vector3.right;
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        moveDir += Vector3.right;
                    }

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

    void Move(Vector3 moveDir)
    {
        transform.position += moveDir * Speed * Time.deltaTime;
        Debug.Log(moveDir);
        anim.SetBool("IsFly", moveDir != Vector3.zero);
        moveDir = Vector3.zero;
    }

    void Turn(Vector3 moveDir)
    {
        //transform.LookAt(transform.position + moveDir);
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
        Move(moveDir);
        Turn(moveDir);
    }
}
