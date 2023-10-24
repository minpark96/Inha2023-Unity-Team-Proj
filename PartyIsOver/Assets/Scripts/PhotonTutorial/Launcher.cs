using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Photon.Tutorial
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte _maxPlayersPerRoom = 4;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject _controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject _progressLabel;

        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string _gameVersion = "1";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool _isConnecting;

        #endregion

        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject PlayerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            Screen.SetResolution(800, 480, false);

            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            _progressLabel.SetActive(false);
            _controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            _progressLabel.SetActive(true);
            _controlPanel.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");
                //PhotonNetwork.JoinRandomRoom();
                GameManager.Instance.JoinLobby();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                Debug.Log("PUN Basics Tutorial/Launcher: ConnectUsingSettings() was called by PUN");
                _isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = _gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (_isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");
                //PhotonNetwork.JoinRandomRoom();
                GameManager.Instance.JoinLobby();
                _isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _progressLabel.SetActive(false);
            _controlPanel.SetActive(true);

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            _isConnecting = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            if (PlayerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    if (PlayerManager.LocalPlayerInstance == null)
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }
                    else
                    {
                        Debug.Log("OnJoinedRoom() else" + PlayerManager.LocalPlayerInstance);
                    }
                }
            }
        }

        #endregion
    }
}