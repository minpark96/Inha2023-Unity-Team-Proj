using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sync : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Public Fields

    public List<Transform> ChildrenTransforms;

    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        for (int i = 1; i < gameObject.transform.childCount; i++)
        {
            ChildrenTransforms.Add(gameObject.transform.GetChild(i));
        }

        if (!photonView.IsMine)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    #endregion

    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            for (int i = 0; i < ChildrenTransforms.Count; i++)
            {
                if (ChildrenTransforms[i] != null)
                {
                    stream.SendNext(ChildrenTransforms[i].localPosition);
                    stream.SendNext(ChildrenTransforms[i].localRotation);
                    stream.SendNext(ChildrenTransforms[i].localScale);
                }
            }
        }
        else
        {
            for (int i = 0; i < ChildrenTransforms.Count; i++)
            {
                if (ChildrenTransforms[i] != null)
                {
                    ChildrenTransforms[i].localPosition = (Vector3)stream.ReceiveNext();
                    ChildrenTransforms[i].localRotation = (Quaternion)stream.ReceiveNext();
                    ChildrenTransforms[i].localScale = (Vector3)stream.ReceiveNext();
                }
            }
        }
    }

    #endregion
}
