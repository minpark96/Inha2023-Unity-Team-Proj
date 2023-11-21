using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;

public class SoundManager 
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.Maxcount];
    //캐싱 역할
    Dictionary<string,AudioClip> _audioClip = new Dictionary<string, AudioClip>();
    AudioMixer audioMixer;

    //사운드 오브젝트 생성
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");

        SceneManagerEx sceneManagerEx = new SceneManagerEx();
        string currentSceneName = sceneManagerEx.GetCurrentSceneName();
        Debug.Log("현재 씬 이름 : " + currentSceneName);
        AudioClip audioClip = null;
        audioMixer = null;
        if (root == null) 
        {

            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

         /*   AudioSource _audioSource = root.AddComponent<AudioSource>();
            audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
            _audioSource.clip = audioClip;
            _audioSource.loop = true;
*/
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            //맥스 카운트라는 애가 있으니 빼줌
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;

            if (currentSceneName == "Launcher")
            {
                audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
                _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);
            }

            if (currentSceneName == "Launcher")
            {
                audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
                _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);
            }

        }
        
    }
    
    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClip.Clear();
    }

    public void Play( string path, Define.Sound type = Define.Sound.Effect, float pitch =1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {

            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    //방금 전에 사용한 내용이 중복되면 Resource로 찾지 않고 캐싱을 하여 사용하여 더 빠르게 사용한다.
    AudioClip GetOrAddAudioClip(string path , Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
        {
            //.Bgm만 관련한 사운드 제어
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            //effect 관련한 사운드 제어
            if (_audioClip.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClip.Add(path, audioClip);
            }
        }
        if (audioClip == null)
            Debug.Log($"AudioClip Missing : {path}");
        

        return audioClip;
    }
}
