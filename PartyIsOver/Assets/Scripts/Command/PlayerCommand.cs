using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommand : ICommand
{
    protected Actor actor;
    protected BodyHandler bodyHandler;
    protected AnimationPlayer animPlayer;
    protected AnimationData animData;
    public float Timestamp { get; set; }

    public PlayerCommand()
    {
        Timestamp = Time.time;
    }

    public virtual bool Execute(in PlayerContext data)
    {
        return true;
    }
}
