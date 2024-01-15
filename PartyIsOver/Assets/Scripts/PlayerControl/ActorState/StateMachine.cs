using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private BaseState currentState;
    public PlayerCharacter PlayerCharacter;

    void Start()
    {
        currentState = GetInitialState();
        if(currentState != null )
        {
            currentState.Enter();
        }
    }

    void Update()
    {
        if(currentState != null)
        {
            currentState.UpdateLogic();
        }
    }

    private void LateUpdate()
    {
        if(currentState != null)
        {
            currentState.UpdatePhysics();
        }
    }

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }
    
}
