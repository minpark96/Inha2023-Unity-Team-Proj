using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCommand
{
    protected Actor actor;
    public float Timestamp { get; set; }

    public PlayerCommand()
    {
        Timestamp = Time.time;
    }

    public virtual bool Execute(in PlayerActionContext data)
    {
        return true;
    }
}
