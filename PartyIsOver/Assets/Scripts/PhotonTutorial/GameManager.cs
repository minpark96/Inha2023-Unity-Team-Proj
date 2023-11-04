using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

namespace Photon.Tutorial
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Privates Fields

        static GameManager _instance;

        #endregion

        #region Public Fields

        public static GameManager Instance { get { return _instance; } }

        [Tooltip("The prefab to use for representing the player")]
        public GameObject PlayerPrefab;

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    Debug.Log("We load the 'Game Room' ");

                    // #Critical
                    // Load the Room Level.
                    LoadArena();
                }
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                //LoadArena();
            }
            else
            {

            }
        }

        #endregion

        #region Public Methods

        public void JoinRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }


        public void JoinLobby()
        {
            SceneManager.LoadScene(1);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        void Start()
        {
            Debug.LogFormat("We are Start Scene {0}", SceneManagerHelper.ActiveSceneName);

            Init();
        }

        static void Init()
        {
            if (_instance == null)
            {
                GameObject _go = GameObject.Find("Game Manager");
                if (_go == null)
                {
                    _go = new GameObject { name = "Game Manager" };
                    _go.AddComponent<GameManager>();
                }
                DontDestroyOnLoad(_go);
                _instance = _go.GetComponent<GameManager>();
            }
            else
            {

            }
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            PhotonNetwork.LoadLevel("Game Room");

            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("LoadArena() else" + PlayerManager.LocalPlayerInstance);
            }
        }

        #endregion
    }
}