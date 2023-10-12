using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private GameObject grabbedObj;
    private Rigidbody rb;
    public int isLeftorRight;
    public bool alreadGrabbing = false;
    FixedJoint fj;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Item"))
        {
            if (Input.GetMouseButton(0))
            {

                if(grabbedObj == null)
                {
                    grabbedObj = other.gameObject;

                    fj = grabbedObj.AddComponent<FixedJoint>();
                    fj.connectedBody = rb;
                    fj.breakForce = 9001;
                }
            }
            else if (grabbedObj != null)
            {
                Destroy(grabbedObj.GetComponent<FixedJoint>());
                fj = null;
                grabbedObj = null;
            }
        }
    }
}
