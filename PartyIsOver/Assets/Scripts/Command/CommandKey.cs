using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandKey
{
    protected Actor actor;
    public float Timestamp { get; set; }

    public CommandKey()
    {
        Timestamp = Time.time;
    }

    public virtual bool Execute(in PlayerActionContext data)
    {
        return true;
    }
}
