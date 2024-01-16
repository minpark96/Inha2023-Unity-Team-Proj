using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMove : ICommand
{
    private Rigidbody rb;
    private Vector3 direction;
    private float speed;

    public CommandMove(Rigidbody rb, Vector3 direction, float speed)
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
