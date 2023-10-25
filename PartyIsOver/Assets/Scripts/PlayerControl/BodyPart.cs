using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{

    public Rigidbody PartRigidbody;
    private CollisionHandler _partCollisionHandler;
    public InteractableObject PartInteractable;




    private void Awake()
    {
        PartRigidbody = GetComponent<Rigidbody>();
        PartInteractable = gameObject.AddComponent<InteractableObject>();
        _partCollisionHandler = gameObject.AddComponent<CollisionHandler>();
    }



    void PartSetup()
    {
            
    }
}
