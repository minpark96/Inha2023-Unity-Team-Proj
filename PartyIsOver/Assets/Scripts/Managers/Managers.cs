using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Photon.Pun;

public class Managers : MonoBehaviourPun
{
     static Managers _instance;
     static Managers Instance { get { return _instance; } }

    DurationTimeManager _durationTime = new DurationTimeManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();

    public static DurationTimeManager DurationTime { get { return Instance._durationTime; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SoundManager Sound { get { return Instance._sound; } }

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

            _instance._pool.Init();
            _instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Pool.Clear();
    }
}