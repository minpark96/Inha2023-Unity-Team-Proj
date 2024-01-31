using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchReady : BodyState
{
    private UpperBodySM _sm;

    public PunchReady(StateMachine stateMachine) : base("PunchReadyState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
