using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpperBodyState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}
