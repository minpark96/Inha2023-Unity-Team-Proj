using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Photon.Pun;

public class Managers : MonoBehaviourPun
{
     static Managers _instance;
     static Managers Instance { get { return _instance; } }

    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    private void Awake()
    {
        Init();

    }

    void Start()
    {
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
            DontDestroyOnLoad(_go);

            //_instance._data.Init();
            _instance = _go.GetComponent<Managers>();
            _instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
    }

}