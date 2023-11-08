using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HandChecker : MonoBehaviourPun
{
    public bool isCheck = false;

    Grab _grab;

    // Start is called before the first frame update
    void Start()
    {
        _grab = transform.root.GetComponent<Grab>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.tag == "ItemHandle" && _grab._isGrabbing)
        {
            _grab.GrabObjectType = Define.GrabObjectType.Item;
            isCheck = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.tag == "ItemHandle")
        {
            _grab.GrabObjectType = Define.GrabObjectType.None;
            isCheck = false;
        }
    }
}
