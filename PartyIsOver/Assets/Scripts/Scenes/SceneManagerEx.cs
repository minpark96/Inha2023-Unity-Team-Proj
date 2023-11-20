using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx 
{
    public BaseScene CurrentScene{ get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        //현제 사용했던 씬을 날려주고 다음 씬으로 이동
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    //Define에 있는 문자열을 뽑아내는 방법이다. LoadScene가 문자열을 받아야하기 때문에 이러한 방법 사용
    public string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene),type);
        return name;
    }

    public string GetCurrentSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }

}
