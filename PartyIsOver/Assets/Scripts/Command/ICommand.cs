using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute(Vector3 moveDir=default);
}
