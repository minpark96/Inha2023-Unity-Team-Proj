using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{

    public Rigidbody PartRigidbody;
    private CollisionHandler _partCollisionHandler;
    public InteractableObject PartInteractable;
    public ConfigurableJoint PartJoint;



    private void Awake()
    {
        PartRigidbody = GetComponent<Rigidbody>();
        PartInteractable = gameObject.AddComponent<InteractableObject>();
        _partCollisionHandler = gameObject.AddComponent<CollisionHandler>();
        PartJoint = gameObject.GetComponent<ConfigurableJoint>();
    }



    void PartSetup()
    {
            
    }
}
