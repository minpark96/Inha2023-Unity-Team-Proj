using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    Button ButtonStart;

    void Start()
    {
        ButtonStart = transform.GetComponent<Button>();
        ButtonStart.onClick.AddListener(PhotonManager.Instance.Connect);
    }
}
