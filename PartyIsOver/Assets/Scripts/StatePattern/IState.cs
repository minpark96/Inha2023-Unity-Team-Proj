using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface IState
{
    public Actor MyActor { get; set;  }
    public float CoolTime { get; set; }

    void EnterState();
    void UpdateState();
    void ExitState();
}
