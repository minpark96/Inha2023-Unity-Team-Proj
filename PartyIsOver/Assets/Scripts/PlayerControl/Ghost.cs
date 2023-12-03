using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;

public class Ghost : MonoBehaviourPunCallbacks
{
    public float Speed = 5.0f;

    public static GameObject LocalGhostInstance;
    public CameraControl CameraControl;

    void Awake()
    {
        if (photonView.IsMine)
        {
            LocalGhostInstance = this.gameObject; 
            
            if (CameraControl == null)
            {
                Debug.Log("카메라 컨트롤 초기화");
                CameraControl = GetComponent<CameraControl>();
            }
        }

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        if (!photonView.IsMine) return;

        switch (evt)
        {

            case Define.KeyboardEvent.Press:
                {
                    Vector3 moveDir = new Vector3(0, 0, 0);
                    if (Input.GetKey(KeyCode.W))
                    {
                        moveDir += Vector3.forward;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        moveDir -= Vector3.right;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        moveDir -= Vector3.forward;
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        moveDir += Vector3.right;
                    }

                    if (moveDir != Vector3.zero)
                    {
                        moveDir = moveDir.normalized * Speed * Time.deltaTime;
                    }

                    transform.position += moveDir;
                    break;
                }
        }
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (!photonView.IsMine) return;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        CameraControl.LookAround(transform.position);
        CameraControl.CursorControl();
    }
}
