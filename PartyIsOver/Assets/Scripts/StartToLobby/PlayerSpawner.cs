using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefabs;
    public Transform[] SpawnPoints;

    private void Start()
    {
        SpawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        int randomNumber = Random.Range(0, SpawnPoints.Length);

        PhotonNetwork.Instantiate(PlayerPrefabs.name, SpawnPoints[randomNumber].position, Quaternion.identity);
    }

}
