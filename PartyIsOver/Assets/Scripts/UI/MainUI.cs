using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class MainUI : MonoBehaviour
{
    public Text NickName;
    public GameObject StartObject;
    public GameObject CancelPanel;
    public GameObject LoadingPanel;
    public Animator Animator;
    public Image ImageHPBar;

    private bool _gameStartFlag;
    private bool _loadingFlag;
    private bool _loadingDelayFlag;
    private float _angle = 0f;
    private float _delayTime = 0.0f;
    private float _loadingTime = 2f;


    private void Start()
    {
        NickName.text = PhotonNetwork.NickName;
        CancelPanel.SetActive(false);
        LoadingPanel.SetActive(false);
    }

    void Update()
    {
        if(_gameStartFlag)
        {
            _angle -= 0.5f;
            StartObject.transform.rotation = Quaternion.Euler(_angle, 180, 0);

            if (_angle <= -90)
            {
                _gameStartFlag = false;
                _loadingFlag = true;
            }
        }

        if (_loadingFlag)
            DelayTime();
    }

    void DelayTime()
    {
        _delayTime += Time.deltaTime;

        if (_delayTime >= 1 && _loadingDelayFlag == false)
        {
            LoadingPanel.SetActive(true);
            _loadingDelayFlag = true;
        }

        ImageHPBar.fillAmount = (_delayTime - 1) / _loadingTime;

        if ((_delayTime - 1) >= _loadingTime)
        {
            _delayTime = 0;
            _gameStartFlag = false;
            _loadingFlag = false;
            PhotonManager.Instance.Connect();
            PhotonManager.Instance.LoadNextScene("[3]Lobby");
            SceneManager.LoadSceneAsync("[3]Lobby");
        }
    }

    public void OnClickStart()
    {
        _gameStartFlag = true;
        Animator.SetBool("Pose", true);
    }
    public void OnClickPopup()
    {
        CancelPanel.SetActive(true);
    }
    public void OnClickPopUpCancel()
    {
        CancelPanel.SetActive(false);
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
