using Photon.Pun;
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

    public void PartSetup(Define.BodyPart part)
    {
        _partCollisionHandler = GetOrAddComponent<CollisionHandler>();
        PartRigidbody = GetOrAddComponent<Rigidbody>();
        PartInteractable = GetOrAddComponent<InteractableObject>();
        PartTransform = transform;
        if(!(part == Define.BodyPart.Hip))
            PartJoint = GetOrAddComponent<ConfigurableJoint>();

        SetPhotonRigidbody();
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        T component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    private void SetPhotonRigidbody()
    {
        PartRigidbody.maxAngularVelocity = 15f;
        PartRigidbody.solverIterations = 12;
        PartRigidbody.solverVelocityIterations = 12;
        var photonView = GetOrAddComponent<PhotonRigidbodyView>();
        photonView.m_SynchronizeVelocity = true;
        photonView.m_SynchronizeAngularVelocity = true;
    }
}
