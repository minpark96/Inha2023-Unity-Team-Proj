using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkState : MonoBehaviour
{
    public PlayerController PlayerController;
    public Transform CameraArm;
    private Actor _actor;

    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
        _actor = GetComponentInParent<Actor>();
        CameraArm = transform.GetChild(0).GetChild(0);
    }



}
