using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetValure : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider audioSlider;

    public void SetLevel(float sliderVal)
    {
        float sound = audioSlider.value;

        if (sound == -40f) mixer.SetFloat("masterVol", -80);
        else mixer.SetFloat("masterVol", sound);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
