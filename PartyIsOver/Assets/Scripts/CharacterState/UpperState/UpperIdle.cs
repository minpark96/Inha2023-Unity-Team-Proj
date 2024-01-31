using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperIdle : BodyState
{
    private UpperBodySM _sm;

    public UpperIdle(StateMachine stateMachine) : base("UpperIdleState", stateMachine)
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
