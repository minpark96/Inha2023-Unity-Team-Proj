using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraControllable
{
    void LookAround(Vector3 position);
    void CursorControl();
}
