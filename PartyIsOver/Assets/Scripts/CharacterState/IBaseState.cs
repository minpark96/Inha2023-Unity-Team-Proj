using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseState
{
   string Name { get; set; }

   void Enter();
   void UpdateLogic();
   void UpdatePhysics();
   void Exit();
}
