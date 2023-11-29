using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverEffect : MonoBehaviour
{
    public float GrayScale = 0.0f;
    public float AppliedTime = 3.0f;
    Material _cameraMeterial;

    // Start is called before the first frame update
    void Start()
    {
        _cameraMeterial = new Material(Shader.Find("Custom/Grayscale"));
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _cameraMeterial.SetFloat("_Grayscale", GrayScale);
        Graphics.Blit(source, destination, _cameraMeterial);
    }

    public void StartGameOverEffect()
    {
        StartCoroutine(ShowGrayScaleEffect());
    }

    private IEnumerator ShowGrayScaleEffect()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < AppliedTime)
        {
            elapsedTime += Time.deltaTime;

            GrayScale = elapsedTime / AppliedTime;
            yield return null;
        }

        //GrayScale = 0.0f;
    }
}
