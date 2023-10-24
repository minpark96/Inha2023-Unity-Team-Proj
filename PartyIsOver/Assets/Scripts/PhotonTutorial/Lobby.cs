using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Photon.Tutorial
{
    public class Lobby : MonoBehaviour
    {
        #region Private Serializable Fields

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private TMP_Text _playerNameText;

        #endregion

        #region Private Fields

        private void Start()
        {
            GetPlayerName();
        }

        #endregion

        #region Public Methods

        public void GetPlayerName()
        {
            Debug.Log("GetPlayerName(): " + PhotonNetwork.NickName);
            _playerNameText.text = PhotonNetwork.NickName;
        }

        #endregion
    }
}