using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviourPun
{
    public Transform CameraArm;
    public Camera Camera;
    
    void Awake()
    {
        if (!photonView.IsMine)
            transform.GetChild(0).gameObject.SetActive(false); // 다른 클라이언트 카메라 끄기

        if (CameraArm == null)
            CameraArm = transform.GetChild(0).GetChild(0);

        if (Camera == null)
            Camera = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Camera>();
    }

    //카메라 컨트롤
    public void LookAround(Vector3 hipPos)
    {
        CameraArm.parent.transform.position = hipPos;

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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
