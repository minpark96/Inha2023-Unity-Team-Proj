using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour를 상속받지 않은 클래스에서 코루틴을 사용하기 위한 클래스
public class CoroutineHelper : MonoBehaviour
{
    private static MonoBehaviour monoInstance;

    [RuntimeInitializeOnLoadMethod]
    private static void Initializer()
    {
        monoInstance = new GameObject($"[{nameof(CoroutineHelper)}]").AddComponent<CoroutineHelper>();
        DontDestroyOnLoad(monoInstance.gameObject);
    }

    public new static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return monoInstance.StartCoroutine(coroutine);
    }

    public new static void StopCoroutine(Coroutine coroutine) 
    {
        monoInstance.StopCoroutine(coroutine);
    }
}
