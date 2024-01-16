using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviourPun ,ICameraControllable
{
    public Transform CameraArm;
    public Camera Camera;
    
    void Awake()
    {
        if (photonView != null && !photonView.IsMine) return;

        if (CameraArm == null)
            CameraArm = GameObject.Find("CameraArm").transform;

        if (Camera == null)
            Camera = Camera.main;
    }

    //카메라 컨트롤
    public void LookAround(Vector3 position)
    {
        CameraArm.position = position;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = CameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        CameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    public void CursorControl()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetMouseButton(0))
                return;
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (!Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
