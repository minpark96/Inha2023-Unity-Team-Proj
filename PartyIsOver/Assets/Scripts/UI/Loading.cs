using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Loading : MonoBehaviour
{
    AsyncOperation async;


    void Start()
    {
        //StartCoroutine(LoadingNextScene(GameManager.Instance.nextSceneName));    
    }

    void Update()
    {
        DelayTime();
    }

    IEnumerator LoadingNextScene(string sceneName)
    {
        // LoadSceneAsync: 바로 씬을 전환하지 않고, 동기화를 한 후 씬을 넘길지 말지 결정함
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while(async.progress < 0.9f) // 0~1
        {
            yield return true;
        }

        while(async.progress >= 0.9f)
        {
            yield return new WaitForSeconds(0.1f);
            if (delayTime > 5.0f) // 5초의 delay를 건 후
                break;
        }

        // 씬을 바꿔줌
        async.allowSceneActivation = true;
    }

    float delayTime = 0.0f;
    void DelayTime()
    {
        delayTime += Time.deltaTime;
        ImageHPBar.fillAmount = delayTime / 5;

    }

    // hp바
    public Image ImageHPBar = null;
}
