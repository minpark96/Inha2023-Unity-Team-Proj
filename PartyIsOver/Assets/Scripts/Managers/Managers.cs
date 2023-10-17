using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
<<<<<<< HEAD:PartyIsOver/Assets/Scripts/Manager/Managers.cs

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    private static Managers Instance { get { return _instance; } }
=======
using Photon.Pun;

public class Managers : MonoBehaviourPun
{
    private static Managers _instance;
    public static Managers Instance { get { return _instance; } }
>>>>>>> a5c093a73652f0cf6725240cee843f5af5f716fe:PartyIsOver/Assets/Scripts/Managers/Managers.cs

    private InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }
    static void Init()
    {
        if (_instance == null)
        {
            GameObject _go = GameObject.Find("@Managers");
            if (_go == null)
            {
                _go = new GameObject { name = "@Managers" };
                _go.AddComponent<Managers>();
            }
<<<<<<< HEAD:PartyIsOver/Assets/Scripts/Manager/Managers.cs

=======
>>>>>>> a5c093a73652f0cf6725240cee843f5af5f716fe:PartyIsOver/Assets/Scripts/Managers/Managers.cs
            DontDestroyOnLoad(_go);
            _instance = _go.GetComponent<Managers>();
        }
    }
<<<<<<< HEAD:PartyIsOver/Assets/Scripts/Manager/Managers.cs
    
=======
>>>>>>> a5c093a73652f0cf6725240cee843f5af5f716fe:PartyIsOver/Assets/Scripts/Managers/Managers.cs
}