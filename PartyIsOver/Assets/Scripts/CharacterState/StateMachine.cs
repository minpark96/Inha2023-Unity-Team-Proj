using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    IBaseState currentState;
    public PlayerInputHandler inputHandler;

    public StateMachine()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    protected void UpdateLogic()
    {
        if (currentState != null)
        {
            currentState.UpdateLogic();

            if (Input.anyKey)
                currentState.GetInput();
        }
    }


    protected void UpdatePhysics()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
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

    private void OnGUI()
    {
        string content = currentState != null ? currentState.Name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
