using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class LauncherUI : MonoBehaviour
{
    private GameObject _controlPanel;
    private GameObject _cancelPanel;

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

    public void OnClickGameStart()
    {
        PhotonManager.Instance.Connect();
        SceneManager.LoadSceneAsync("Main");
    }
    public void OnClickPopup()
    {
        _cancelPanel.SetActive(true);
    }
    public void OnClickPopUpCancel()
    {
        _cancelPanel.SetActive(false);
    }
    public void OnClickPopUpGameQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
