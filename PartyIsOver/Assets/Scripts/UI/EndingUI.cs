using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndingUI : MonoBehaviour
{
    public GameObject PopUpPanel;
    public GameObject Winner;
    public Text WinnerName;

    private void Start()
    {
        PopUpPanel.SetActive(false);
        Winner = GameObject.Find("Winner");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        AudioSource endingBgm = Managers.Sound.GetBgmAudioSource();
        AudioClip audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/Runaway Train LOOPING");
        endingBgm.clip = audioClip;
        Managers.Sound.Play(audioClip, Define.Sound.Bgm);

        StartCoroutine(PopUp());
    }

    IEnumerator PopUp()
    {
        yield return new WaitForSeconds(5.0f);

        PopUpPanel.SetActive(true);
    }

    public void OnClickMain()
    {
        AudioSource endingBgm = Managers.Sound.GetBgmAudioSource();
        AudioClip audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/LaxLayoverLOOPING");
        endingBgm.clip = audioClip;
        endingBgm.volume = Managers.Sound.SoundVolume[(int)Define.Sound.Bgm];
        Managers.Sound.Play(audioClip, Define.Sound.Bgm);

        PhotonManager.Instance.LeaveRoom();
    }

    public void OnClickCancel()
    {
        PopUpPanel.SetActive(false);
        StartCoroutine(PopUp());
    }

    public void SetWinner(int winner)
    {
        Winner.transform.GetChild(winner).gameObject.SetActive(true);
    }

}
