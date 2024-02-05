using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    float Timestamp { get; set; }
    bool Execute(in Define.PlayerDynamicData data);
    //void Execute(IPlayerContext context);

}
