using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILowerBodyState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}
