using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour , IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }

    public void EnterState()
    {
        effectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {

    }
    public void InstantiateEffect(string path)
    {

    }
    public void RemoveObject(string name)
    {

    }
}
