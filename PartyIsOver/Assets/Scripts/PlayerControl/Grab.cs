using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviourPun
{
    private GameObject _grabGameObject;
    private Rigidbody _grabRigidbody;

    private bool _isGrabbing = false;
    private FixedJoint _joint;
    public PhotonView PV;

    void Start()
    {
        _grabRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        photonView.RPC("Grap", RpcTarget.All, other);
    }

    [PunRPC]
    private void Grap(Collider other)
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
