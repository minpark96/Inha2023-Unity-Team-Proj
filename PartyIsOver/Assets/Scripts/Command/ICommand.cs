using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    float Timestamp { get; set; }
    void Execute(in Define.PlayerDynamicData data);
    //void Execute(IPlayerContext context);

}
