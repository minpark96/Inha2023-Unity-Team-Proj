using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Floor : MonoBehaviour
{
    MagneticField _magneticField;
    int _beforeAreaName;

    public delegate void CheckArea(int[] areaName, int index);
    public event CheckArea CheckFloor;

    private void Start()
    {
        _magneticField = GameObject.Find("Magnetic Field").GetComponent<MagneticField>();
        _beforeAreaName = (int)Define.Area.Default;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.IsConnected == true) return;
        if (_magneticField.ActorList == null) return;


        for (int i = 0; i < _magneticField.ActorList.Count; i++)
        {
            if (other.name == _magneticField.ActorList[i].BodyHandler.LeftLeg.transform.GetChild(0).name)
            {
                Transform collided = other.gameObject.transform.parent.parent;

                if (collided.name == _magneticField.ActorList[i].name)
                {
                    if (_beforeAreaName == (int)Define.Area.Floor)
                        return;

                    _beforeAreaName = _magneticField.AreaNames[i];
                    _magneticField.AreaNames[i] = (int)Define.Area.Floor;
                    CheckFloor(_magneticField.AreaNames, i);
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.IsConnected == true) return;
        if (_magneticField.ActorList == null) return;


        for (int i = 0; i < _magneticField.ActorList.Count; i++)
        {
            if (other.name == _magneticField.ActorList[i].BodyHandler.LeftLeg.transform.GetChild(0).name)
            {
                Transform collided = other.gameObject.transform.parent.parent;

                if (collided.name == _magneticField.ActorList[i].name)
                {
                    _beforeAreaName = _magneticField.AreaNames[i];
                    _magneticField.AreaNames[i] = (int)Define.Area.Default;
                    CheckFloor(_magneticField.AreaNames, i);
                    break;
                }
            }
        }
    }
}
