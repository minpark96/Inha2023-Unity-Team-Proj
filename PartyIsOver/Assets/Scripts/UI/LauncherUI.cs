using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.Video;


public class LauncherUI : MonoBehaviour
{
    private GameObject _startPanel;
    private GameObject _controlPanel;
    private GameObject _cancelPanel;
    private GameObject _errorPanel;
    private GameObject _feedbackPanel;
    private Regex specialRegex = new Regex(@"[~!@\#$%^&*\()\=+|\\/:;?""<>'\[\].,]");
    private GameObject _bgmSound;

    private string _nickName;
    private string _mainSceneName = "[2]Main";
    
    public Text ErrorText;
    public VideoPlayer Video;
    public GameObject PlaceHolder;
    public InputField InputField;

  
    void Start()
    {
        _startPanel = GameObject.Find("Start Panel");
        _controlPanel = GameObject.Find("Control Panel");
        _cancelPanel = GameObject.Find("Cancel Panel");
        _errorPanel = GameObject.Find("Error Panel");
        _feedbackPanel = GameObject.Find("Feedback Panel");

        _controlPanel.SetActive(false);
        _cancelPanel.SetActive(false);
        _errorPanel.SetActive(false);
        _feedbackPanel.SetActive(false);

        PlaceHolder.SetActive(true);
        InputField.onValueChanged.AddListener(SetPlayerName);

        Screen.SetResolution(960, 540, false);

        Video.loopPointReached += EndReached;
        _bgmSound = GameObject.Find("@Sound").transform.GetChild((int)Define.Sound.Bgm).gameObject;
        _bgmSound.GetComponent<AudioSource>().Stop();
    }

   
    public void EndReached(UnityEngine.Video.VideoPlayer video)
    {
        _startPanel.SetActive(false);
        _controlPanel.SetActive(true);
        _bgmSound.GetComponent<AudioSource>().Play();
    }

    void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            PlaceHolder.SetActive(true);
        }

        PlaceHolder.SetActive(false);

        PhotonNetwork.NickName = value;
    }

    public void OnClickGameStart()
    {
        if (PhotonNetwork.NickName.Length < 2 || PhotonNetwork.NickName.Length > 13)
        {
            AudioClip uiSound1 = Managers.Sound.GetOrAddAudioClip("UI/Cartoon-UI-092");
            Managers.Sound.Play(uiSound1, Define.Sound.UISound);

            _errorPanel.SetActive(true);
            ErrorText.text = "닉네임 글자 수가 너무 적거나 많습니다.";
            return;
        }

        if (specialRegex.IsMatch(PhotonNetwork.NickName))
        {
            AudioClip uiSound1 = Managers.Sound.GetOrAddAudioClip("UI/Cartoon-UI-092");
            Managers.Sound.Play(uiSound1, Define.Sound.UISound);

            _errorPanel.SetActive(true);
            ErrorText.text = "사용 불가능한 특수 문자가 포함되어 있습니다.";
            return;
        }

        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _nickName = PhotonNetwork.NickName;
        _feedbackPanel.SetActive(true);
        _feedbackPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _nickName;
    }

    public void OnClickFeedbackOK()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-160");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        Video.loopPointReached -= EndReached;

        PhotonManager.Instance.Connect();
        SceneManager.LoadSceneAsync(_mainSceneName);
    }

    public void OnClickFeedbackPanelCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _feedbackPanel.SetActive(false);
    }

    public void OnClickErrorOK()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _errorPanel.SetActive(false);
    }

    public void OnClickCancelPanel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _cancelPanel.SetActive(true);
    }

    public void OnClickCancelPanelCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _cancelPanel.SetActive(false);
    }

    public void OnClickGameQuit()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    // 빌드시 삭제할 부분
    public void OnClickShortcut()
    {
        PhotonManager.Instance.Connect();
        PhotonManager.Instance.LoadNextScene("[3]Lobby");
        SceneManager.LoadSceneAsync("[3]Lobby");
    }
}
