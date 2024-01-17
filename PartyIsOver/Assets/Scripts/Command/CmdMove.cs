using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdMove : ICommand
{
    private Rigidbody rb;
    private Vector3 direction;
    private float speed;

    public CmdMove(Rigidbody rb, Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
        this.rb = rb;
    }

    public void Execute(Vector3 moveDir = default)
    {
        rb.AddForce(direction);
    }
}
