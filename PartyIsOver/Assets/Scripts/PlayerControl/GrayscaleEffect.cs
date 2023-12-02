using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

public class GrayscaleEffect : MonoBehaviour
{
    public Shader GrayscaleShader;
    Material grayscaleMaterial; 
    public float TransitionTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        grayscaleMaterial = new Material(GrayscaleShader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, grayscaleMaterial);
    }

    public void StartGrayscalseEffect()
    {
        Debug.Log("StartGrayscalseEffect!!!!!!");
        StartCoroutine(TransitToGrayScale());
    }

    IEnumerator TransitToGrayScale()
    {
        float timer = 0.0f;
        while (timer < TransitionTime)
        {
            timer += Time.deltaTime;
            float lerpFactor = timer / TransitionTime;
            grayscaleMaterial.SetFloat("_LerpFactor", lerpFactor);
            //Debug.Log("_LerpFactor: " + lerpFactor); // _LerpFactor 값을 로그로 출력
            yield return null;
        }
        grayscaleMaterial.SetFloat("_LerpFactor", 1.0f);
    }
}
