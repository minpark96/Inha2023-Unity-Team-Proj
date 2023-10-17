using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    public Transform targetLimb; // target
    public bool mirror;
    ConfigurableJoint cj; // mine

    Quaternion initialRotation;



    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        //initialRotation = cj.targetRotation;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRotationDifference = Quaternion.Inverse(initialRotation) * targetLimb.rotation;

        if (!mirror)
        {
            cj.targetRotation = targetRotationDifference;
        }
        else
        {
            cj.targetRotation = Quaternion.Inverse(targetRotationDifference);
        }
    }
}
