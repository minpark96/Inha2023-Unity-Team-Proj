using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class BaseScene : MonoBehaviourPunCallbacks
{
    Define.Scene _sceneType = Define.Scene.Unknown;

    public Define.Scene SceneType { get;protected set; } = Define.Scene.Unknown;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        //GameScene에서 UI 생성하는 코드
        //Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        //if(obj == null)
        //  Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();

}
