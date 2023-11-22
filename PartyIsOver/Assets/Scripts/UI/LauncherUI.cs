using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class LauncherUI : PhotonManager
{
    GameObject _controlPanel;
    GameObject _cancelPanel;
    GameObject _loadingPanel;

    string _gameVersion = "1";

    bool flag;
    float delayTime = 0.0f;

    AsyncOperation async;
    public Image ImageHPBar;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _controlPanel = GameObject.Find("Control Panel");
        _cancelPanel = GameObject.Find("Cancel Panel");
        _loadingPanel = GameObject.Find("Loading Panel");

        _controlPanel.SetActive(true);
        _cancelPanel.SetActive(false);
        _loadingPanel.SetActive(false);
    }

    public void Connect()
    {
        _loadingPanel.SetActive(true);
        flag = true;

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");

            // 일단 Room, Join Lobby가 맞는듯
            //PhotonNetwork.JoinRandomRoom();

            //_controlPanel.SetActive(false);
            //_cancelPanel.SetActive(false);


            if(!flag)
                PhotonNetwork.JoinLobby();
        }
        else
        {
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }

        SceneManagerEx sceneManagerEx = new SceneManagerEx();
        string currentSceneName = sceneManagerEx.GetCurrentSceneName();
        if (currentSceneName == _roomSceneName)
        {
            AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.Maxcount];
            AudioClip audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
            _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
            Managers.Sound.Play(audioClip, Define.Sound.Bgm);
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[JoinLobby()] Load Lobby Scene");
        //SceneManager.LoadScene(2);
        //InstantiateGameCenter();

        //if (PhotonNetwork.IsMasterClient)
        //{
        StartCoroutine(LoadAsyncScene(_roomSceneName));
        //}
    }



    void Update()
    {
        if(flag)
            DelayTime();
    }

    

    void DelayTime()
    {
        delayTime += Time.deltaTime;
        ImageHPBar.fillAmount = delayTime / 5;

        if (delayTime == 5)
            flag = false;
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
