using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnOff : MonoBehaviour
{
    public BoxCollider MainCollider;
    public GameObject ThisGuysRig;
    public Animator ThisGuysAnimator;
    public Rigidbody ThisGuysRigidbody;


    private void Start()
    {
        GetRagdollBits();
        RagdollModeOff();
    }

    private void Update()
    {
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            RagdollModeOn();
        }
        Debug.Log("Enter");
    }

    private void OnCollisionExit(Collision collision)
    {
        RagdollModeOff();
        Debug.Log("Exit");

    }

    IEnumerator GetReady()
    {
        yield return new WaitForSeconds(3.0f);
    }

    Collider[] RagDollColliders;
    Rigidbody[] LimbsRigidbodies;

    // Ragdoll에 원래 붙어있던 collider, rigidbody들
    void GetRagdollBits()
    {
        RagDollColliders = ThisGuysRig.GetComponentsInChildren<Collider>();
        LimbsRigidbodies = ThisGuysRig.GetComponentsInChildren<Rigidbody>();
    }

    void RagdollModeOn()
    {
        // Ragdoll 켜기
        ThisGuysAnimator.enabled = false;

        foreach (Collider col in RagDollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in LimbsRigidbodies)
        {
            rigid.isKinematic = false;
        }

        MainCollider.enabled = false;
        ThisGuysRigidbody.isKinematic = true;
    }

    
    void RagdollModeOff()
    {
        // Ragdoll 끄기

        foreach(Collider col in RagDollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rigid in LimbsRigidbodies)
        {
            rigid.isKinematic = true;
        }

        ThisGuysAnimator.enabled = true;
        MainCollider.enabled = true;
        ThisGuysRigidbody.isKinematic = false;
    }



}
