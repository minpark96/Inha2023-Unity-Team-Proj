using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    //MagneticField _magneticField;
    //int _areaName;
    //int _beforeAreaName;

    //public delegate void CheckArea(int areaName);
    //public event CheckArea CheckMagneticFieldArea;

    //private void Start()
    //{
    //    _magneticField = GameObject.Find("Magnetic Field").GetComponent<MagneticField>();
    //    _beforeAreaName = _magneticField.AreaName;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (!PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.IsConnected == true) return;

    //    if (_magneticField.Actor == null) return;

        
    //    if (other.name == _magneticField.Actor.BodyHandler.LeftLeg.transform.GetChild(0).name)// || other.name == _magneticField.Actor.BodyHandler.RightLeg.transform.GetChild(0).name)
    //    {
    //        _beforeAreaName = _magneticField.AreaName;

    //        if (_beforeAreaName == (int)Define.Area.Floor) 
    //            return;
    //        else
    //        {
    //            _areaName = (int)Define.Area.Floor;
    //            _magneticField.AreaName = _areaName;
    //            CheckMagneticFieldArea(_areaName);
    //        }
    //    }
    //}
}
