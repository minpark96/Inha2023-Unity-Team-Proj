using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    private CollisionHandler _partCollisionHandler;

    public Rigidbody PartRigidbody;
    public InteractableObject PartInteractable;
    public ConfigurableJoint PartJoint;
    public Transform PartTransform;
    public RigidbodyConstraints PartRigidbodyConstraints;

    private void Awake()
    {
        _partCollisionHandler = gameObject.AddComponent<CollisionHandler>();

        PartRigidbody = GetComponent<Rigidbody>();
        PartInteractable = gameObject.AddComponent<InteractableObject>();
        PartJoint = GetComponent<ConfigurableJoint>();
        PartTransform = GetComponent<Transform>();
        PartRigidbodyConstraints = PartRigidbody.constraints;
    }



    void PartSetup()
    {
            
    }
}
