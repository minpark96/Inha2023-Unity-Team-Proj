using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    IBaseState currentState;
    public PlayerInputHandler inputHandler;


    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateLogic();

            if (Input.anyKey)
                currentState.GetInput();
        }
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

    private void OnGUI()
    {
        string content = currentState != null ? currentState.Name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
