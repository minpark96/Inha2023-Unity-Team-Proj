using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    IBaseState currentState;


    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    private void FixedUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }

    void LateUpdate()
    {
       
    }

    protected virtual IBaseState GetInitialState()
    {
        return null;
    }

    public void ChangeState(IBaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
