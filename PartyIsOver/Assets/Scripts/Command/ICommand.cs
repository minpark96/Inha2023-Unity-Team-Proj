using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    float Timestamp { get; set; }
    bool Execute(in PlayerContext data);
    //void Execute(IPlayerContext context);

}
