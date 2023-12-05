using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EndingUI : MonoBehaviour
{
    public GameObject PopUpPanel;
    public GameObject Winner;
    public int WinnerNumber;

    private void Start()
    {
        PopUpPanel.SetActive(false);
        Winner = GameObject.Find("Winner");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(PopUp());
    }

    IEnumerator PopUp()
    {
        yield return new WaitForSeconds(5.0f);

        PopUpPanel.SetActive(true);
    }

    public void OnClickMain()
    {
        SceneManager.LoadScene("[2]Main");
        PhotonManager.Instance.Connect();
        
    }

    public void OnClickCancel()
    {
        PopUpPanel.SetActive(false);
        StartCoroutine(PopUp());
    }

}
