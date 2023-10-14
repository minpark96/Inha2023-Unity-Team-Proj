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

    Vector3 moveInput;
    Vector3 moveDir;
    private float _rotationSpeed = 300.0f;


    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveInput.magnitude != 0)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            moveDir = lookForward * moveInput.z + lookRight * moveInput.x;


            Vector3 forward = Vector3.Slerp(-hips.transform.up, moveDir, _rotationSpeed * Time.deltaTime / Vector3.Angle(-hips.transform.up, moveDir));
           
            Debug.Log(hips.transform.position + forward);
            //Debug.Log(hips.transform.position);
            //Debug.Log(forword);

            //hips.transform.LookAt(hips.transform.position + forword);
            //hips.transform.up = (hips.transform.position - forword).normalized;

            //AlignToVector(hips, hips.transform.forward, Vector3.up + moveDir, 0.1f, 4f );
            AlignToVector(hips, -hips.transform.up, Vector3.up + moveDir, 0.1f, 4f * 10f);



            if (Input.GetKey(KeyCode.LeftShift))
            {
                hips.AddForce(moveDir.normalized * speed * Time.deltaTime* 2f);

            }
            else
            {
                hips.AddForce(moveDir.normalized * speed * Time.deltaTime);
                
            }
        }


        
        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded)
            {
                hips.AddForce(new Vector3(0,jumpForce,0));
                //isGrounded = false;
            }
        }



    }



    private void Update()
    {
        LookAround();
        CursorControll();

    }



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
