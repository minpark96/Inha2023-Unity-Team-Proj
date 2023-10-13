using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("앞뒤 속도")]
    public float speed;
    [Header("점프 힘")]
    public float jumpForce;

    [SerializeField]
    private Rigidbody hips;
    public bool isGrounded;

    public Transform cameraArm;
    public Transform forward;

    Vector3 vforward;
    Vector3 moveInput;
    Vector3 moveDir;
    private float _rotationSpeed = 30.0f;


    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        vforward = (forward.position - hips.transform.position).normalized;

        if (moveInput.magnitude != 0)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            moveDir = lookForward * moveInput.z + lookRight * moveInput.x;


            Vector3 forword = Vector3.Slerp(vforward, moveDir, _rotationSpeed * Time.deltaTime / Vector3.Angle(vforward, moveDir));
           
           // Debug.Log(forword);
            //hips.transform.LookAt(hips.transform.position + forword);
            //hips.transform.up = (hips.transform.position + forword).normalized;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                hips.AddForce(moveDir.normalized * speed * Time.deltaTime* 2f);

            }
            else
            {
                hips.AddForce(moveDir.normalized * speed * Time.deltaTime);
                
            }
        }


        
        if(Input.GetAxis("Jump") > 0)
        {
            if(isGrounded)
            {
                hips.AddForce(new Vector3(0,jumpForce,0));
                isGrounded = false;
            }
        }


        ray = new Ray();
        ray.origin = hips.position; //레이의 시작점
        ray.direction = -hips.transform.up;

    }

    private Ray ray; //어느 방향으로 쏠지에 대한 정보를 가짐
    public float distance = 20f;

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(ray.origin, ray.direction * distance + ray.origin);

    }


        private void Update()
    {
        LookAround();
        CursorControll();

    }





    private void LookAround()
    {
        cameraArm.parent.transform.position = hips.transform.position;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }


    // 마우스 컨트롤 온오프
    private void CursorControll()
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
