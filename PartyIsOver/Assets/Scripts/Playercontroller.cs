using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    [Header("앞뒤 속도")]
    public float speed;
    [Header("좌우 속도")]
    public float strafeSpeed;
    [Header("점프 힘")]
    public float jumpForce;

    private GameObject hip;
    private Rigidbody hips;
    public bool isGrounded;

    void Start()
    {
        hip = GameObject.Find("pelvis");
        hips = hip.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W))
            if(Input.GetKey(KeyCode.LeftShift))
                hips.AddForce(transform.forward * speed * 2f);
            else
                hips.AddForce(transform.forward * speed);

        if (Input.GetKey(KeyCode.S))
            if (Input.GetKey(KeyCode.LeftShift))
                hips.AddForce(-transform.forward * speed * 2f);
            else
                hips.AddForce(-transform.forward * speed);

        if (Input.GetKey(KeyCode.A))
            if (Input.GetKey(KeyCode.LeftShift))
                hips.AddForce(-transform.right * strafeSpeed * 2f);
            else
                hips.AddForce(-transform.right * strafeSpeed);

        if (Input.GetKey(KeyCode.D))
            if (Input.GetKey(KeyCode.LeftShift))
                hips.AddForce(transform.right * strafeSpeed * 2f);
            else
                hips.AddForce(transform.right * strafeSpeed);

        if(Input.GetAxis("Jump") > 0)
        {
            if(isGrounded)
            {
                hips.AddForce(new Vector3(0,jumpForce,0));
                isGrounded = false;
            }
        }
            

    }
}
