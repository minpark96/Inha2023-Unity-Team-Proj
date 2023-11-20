using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LauncherUI : MonoBehaviour
{
    GameObject _controlPanel;
    GameObject _cancelPanel;
  
    void Awake()
    {
        Init();
    }

    void Init()
    {
        _controlPanel = GameObject.Find("Control Panel");
        _cancelPanel = GameObject.Find("Cancel Panel");

        _controlPanel.SetActive(true);
        _cancelPanel.SetActive(false);


    }

    public void LauncherCancel()
    {
        _cancelPanel.SetActive(true);
    }

    public void LauncherPopUpCancel()
    {
        _cancelPanel.SetActive(false);
    }

    public void LauncherQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
