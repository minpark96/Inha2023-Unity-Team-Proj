using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveButton : MonoBehaviour
{
    Button ButtonLeave;

    void Start()
    {
        ButtonLeave = transform.GetComponent<Button>();
        ButtonLeave.onClick.AddListener(PhotonManager.Instance.LeaveRoom);
    }
}
