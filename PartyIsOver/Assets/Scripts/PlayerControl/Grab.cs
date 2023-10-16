using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private GameObject _grabGameObject;
    private Rigidbody _grabRigidbody;

    private bool _isGrabbing = false;
    private FixedJoint _joint;
    void Start()
    {
        _grabRigidbody = GetComponent<Rigidbody>();
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
                if (_grabGameObject == null)
                {
                    _grabGameObject = other.gameObject;

                    _joint = _grabGameObject.AddComponent<FixedJoint>();
                    _joint.connectedBody = _grabRigidbody;
                    _joint.breakForce = 9001;
                }
            }
            else if (_grabGameObject != null)
            {
                Destroy(_grabGameObject.GetComponent<FixedJoint>());
                _joint = null;
                _grabGameObject = null;
            }
        }
    }
}
