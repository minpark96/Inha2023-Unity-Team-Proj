using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Photon.Pun;

public class Managers : MonoBehaviourPunCallbacks
{
     static Managers _instance;
     static Managers Instance { get { return _instance; } }

    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    //PhotonManager _photon = new PhotonManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    //public static PhotonManager Photon { get { return Instance._photon; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    void Init()
    {
        if (_instance == null)
        {
            GameObject _go = GameObject.Find("@Managers");
            if (_go == null)
            {
                _go = new GameObject { name = "@Managers" };
                _go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(_go);
            _instance = _go.GetComponent<Managers>();

            //_instance._data.Init();
            //_instance._photon.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Clear()
    {
        //Scene.Clear();
    }

}