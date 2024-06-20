using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Transform teamASpawnPoint;
    public Transform teamBSpawnPoint;

    private void Awake()
    {
        SpawnPlayers();
    }
    void Start()
    {
        
    }

    /// <summary>
    /// ���� ���缭 ������Ŵ > ���� ��ġ�� �������� ���� �� �־ �̷��� ������
    /// </summary>
    private void SpawnPlayers()
    {
        int teamID = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        Transform spawnPoint = (teamID == 1) ? teamASpawnPoint : teamBSpawnPoint;
        PhotonNetwork.Instantiate("PlayerPrefab", spawnPoint.position, spawnPoint.rotation, 0);
        Transform[] Apoints = GameObject.Find("TeamASpawnPoint").GetComponentsInChildren<Transform>();
        Transform[] Bpoints = GameObject.Find("TeamBSpawnPoint").GetComponentsInChildren<Transform>();

        int Aindex = Random.Range(0, Apoints.Length);
        int Bindex = Random.Range(0, Bpoints.Length);

        if (teamID == 1)
        {
            PhotonNetwork.Instantiate("TeamA", Apoints[Aindex].position, Apoints[Aindex].rotation, 0);
        }

        if (teamID == 2)
        {
            PhotonNetwork.Instantiate("TeamB", Apoints[Bindex].position, Apoints[Bindex].rotation, 0);
        }
    }
}