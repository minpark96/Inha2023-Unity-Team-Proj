using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private BaseState currentState;

    void Start()
    {
        Debug.Log("Not null");
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
            Debug.Log("Not null");
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

    private void OnGUI()
    {
        string content = currentState != null ? currentState.name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
    
}
