using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static Actor;

public class Ghost : MonoBehaviourPunCallbacks
{
    public static GameObject LocalGhostInstance;

    // Start is called before the first frame update
    void Awake()
    {
        if (photonView.IsMine)
        {
            LocalGhostInstance = this.gameObject;
        }

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
        
    }
}
