using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public interface IBaseState
{
   Define.PlayerState Name { get; set; }

   void GetInput();
   void Enter();
   void UpdateLogic();
   void UpdatePhysics();
   void Exit();
}
