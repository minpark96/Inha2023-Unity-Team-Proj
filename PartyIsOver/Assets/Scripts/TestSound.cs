using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public AudioClip audioClip;
    private void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();
        //사운드 위치에 따라 소리를 입력
        //audio.PlayClipAtPoint();

        /*AudioSource audio = GetComponent<AudioSource>();
        audio.PlayOneShot(audioClip);
        //오디오가 두개 면은 긴 것을 재생을 하고 오브젝트를 삭제 한다.
        float lifeTime = Mathf.Max(audioClip.length, 0.5f);
        GameObject.Destroy(gameObject, lifeTime);*/

        Managers.Sound.Play(audioClip, Define.Sound.Bgm);

    }

}
