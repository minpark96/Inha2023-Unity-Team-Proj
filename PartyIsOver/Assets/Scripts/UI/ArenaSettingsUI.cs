using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ArenaSettingsUI : MonoBehaviour
{
    private GameObject _settingsPanel;
    private GameObject _cancelPanel;
    private GameObject _soundPanel;
    private bool _isClicked;

    public Slider BGMAudioSlider;
    public Slider EffectAudioSlider;


    void Start()
    {
        _settingsPanel = GameObject.Find("Settings Panel");
        _cancelPanel = GameObject.Find("Cancel Panel");
        _soundPanel = GameObject.Find("Sound Panel");

        _settingsPanel.SetActive(false);
        _cancelPanel.SetActive(false);
        _soundPanel.SetActive(false);

        Managers.Input.KeyboardAction -= OnKeyboardEvents;
        Managers.Input.KeyboardAction += OnKeyboardEvents;
    }


    void OnKeyboardEvents(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickSettingsPanel();
                    }
                }
                break;
        }
    }

    void SetSettingsActive()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _settingsPanel.SetActive(true);
    }

    void SetSettingsInactive()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _settingsPanel.SetActive(false);
    }


    public void OnClickSettingsPanel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);

        _isClicked = !_isClicked;
        if (_isClicked)
            SetSettingsActive();
        else
            SetSettingsInactive();
    }

    #region Sound Settings
    public void OnClickSettings()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);
        _soundPanel.SetActive(true);
    }

    public void OnClickSettingsOK()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);

        Managers.Sound.SoundVolume[(int)Define.Sound.Bgm] = 0.1f * BGMAudioSlider.value;
        Managers.Sound.SoundVolume[(int)Define.Sound.UIInGameSound] = 1f * EffectAudioSlider.value;
        Managers.Sound.ChangeVolumeInArena();
        _soundPanel.SetActive(false);
    }

    public void OnClickSettingsCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);

        Managers.Sound.SoundVolume[(int)Define.Sound.Bgm] = 0.1f * 1;
        Managers.Sound.SoundVolume[(int)Define.Sound.UIInGameSound] = 1f * 1;
        Managers.Sound.ChangeVolumeInArena();

        BGMAudioSlider.value = 1;
        EffectAudioSlider.value = 1;
        _soundPanel.SetActive(false);
    }


    #endregion

    #region Game Quit
    public void OnClickCancelPanel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);
        _cancelPanel.SetActive(true);
    }

    public void OnClickGameQuitOK()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);
        _cancelPanel.SetActive(false);

        // [3]Lobby ·Î ¿¬°á
        Managers.Input.KeyboardAction -= OnKeyboardEvents;

    }

    public void OnClickGameQuitCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UIInGameSound);
        _cancelPanel.SetActive(false);
    }

    #endregion
}
