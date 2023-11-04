using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sync : MonoBehaviourPunCallbacks
{
    #region Public Fields

    //public List<Transform> ChildrenTransforms;
    //public float LerpSpeed = 10f;

    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        //for (int i = 1; i < gameObject.transform.childCount; i++)
        //{
        //    ChildrenTransforms.Add(gameObject.transform.GetChild(i));
        //}

        if (!photonView.IsMine)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    #endregion

    #region IPunObservable

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        for (int i = 0; i < ChildrenTransforms.Count; i++)
    //        {
    //            if (ChildrenTransforms[i] != null)
    //            {
    //                stream.SendNext(ChildrenTransforms[i].localPosition);
    //                stream.SendNext(ChildrenTransforms[i].localRotation);
    //                //stream.SendNext(ChildrenTransforms[i].localScale);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < ChildrenTransforms.Count; i++)
    //        {
    //            if (ChildrenTransforms[i] != null)
    //            {
    //                Vector3 localPosition = (Vector3)stream.ReceiveNext();
    //                Quaternion localRotation = (Quaternion)stream.ReceiveNext();
    //                //Vector3 localScale = (Vector3)stream.ReceiveNext();

    //                ChildrenTransforms[i].localPosition = Vector3.Lerp(ChildrenTransforms[i].localPosition, localPosition, LerpSpeed * Time.deltaTime);
    //                ChildrenTransforms[i].localRotation = Quaternion.Lerp(ChildrenTransforms[i].localRotation, localRotation, LerpSpeed * Time.deltaTime);
    //            }
    //        }
    //    }
    //}

    #endregion
}
