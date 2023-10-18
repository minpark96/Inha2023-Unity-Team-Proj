using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{

    public Rigidbody PartRigidbody;





    private void Awake()
    {
        PartRigidbody = GetComponent<Rigidbody>();
        gameObject.AddComponent<InteractableObject>();
    }



    void PartSetup()
    {
            
    }
}
