using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviourPun
{
    private GameObject _grabGameObject;
    private Rigidbody _grabRigidbody;

    private Vector3 _hipPosition;
    private Quaternion _hipRotation;

    PhotonManager _pm;

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
        if (other.gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            Debug.Log("player2 grab");
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
