using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSettingsUI : MonoBehaviour
{
    private GameObject _settingsPanel;
    private bool _isClicked;

    void Start()
    {
        _settingsPanel = GameObject.Find("Settings Panel");
    }


    void OnKeyboardEvents(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickReturn();
                    }
                }
                break;
        }
    }

    void SetSettingsActive()
    {
        _settingsPanel.SetActive(true);
    }

    void SetScoreBoardInactive()
    {
        _settingsPanel.SetActive(false);
    }


    public void OnClickReturn()
    {
        _isClicked = !_isClicked;
        if (_isClicked)
            SetSettingsActive();
        else
            SetScoreBoardInactive();
    }

    public void OnClickSettings()
    {

    }

    public void OnClickGameExit()
    {

    }

}
