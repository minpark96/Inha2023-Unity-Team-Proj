using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class BaseScene : MonoBehaviourPunCallbacks
{
    Define.SceneType _sceneType = Define.SceneType.Unknown;

    public Define.SceneType SceneType { get; protected set; } = Define.SceneType.Unknown;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        //GameScene     UI      ϴ   ڵ 
        //Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        //if(obj == null)
        //  Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();

}