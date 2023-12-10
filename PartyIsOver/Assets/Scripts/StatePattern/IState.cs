using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface IDebuffState
{
    public Actor MyActor { get; set;  }
    public float CoolTime { get; set; }

    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }

    void EnterState();
    void UpdateState();
    void ExitState();
    public void InstantiateEffect(string path);
    public void RemoveObject(string name);
}
