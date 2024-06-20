using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using TMPro;

public class GameManager : SingletonOfT<GameManager>, IPunObservable
{
    /// <summary>
    /// ������ ���� �Ǹ� true, ���� ���� ���� �� false
    /// </summary>
    public bool isPlaying;

    public static float teamAScore;
    public static float teamBScore;

    [SerializeField] private float _goalScore;

    /// <summary>
    /// �¸� ���� �˸��� string��
    /// </summary>
    public string winningTeam;
    public float endTime;
    public float currentTime;

    public int maxPlayersPerTeam = 5; // �� ���� �ִ� �÷��̾� �� ����
    public GameObject gameCanvas;
    public Button startGameButton; // ���� ���� ��ư
    private string _currentTeam;
    private readonly byte GameStartEventCode = 0;
    private readonly byte HitEventCode = 1;
    public TMP_Text time;
    public Scrollbar aScrollbar;
    public Scrollbar bScrollbar;
    private void Awake()
    {
        Init();
        GameReset();
        ResetWinningTeam();
        OpenRoom();
    }

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += HitEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived -= HitEvent;
    }



    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == GameStartEventCode)
        {
            HandleGameStart();
        }
    }
    public void HitEvent(EventData photonEvent)
    {
        if (photonEvent.Code == HitEventCode)
        {

        }
    }

    private void HandleGameStart()
    {
        gameCanvas.SetActive(false);
        isPlaying = true;
        CreatePlayer();
    }

    public void JoinTeamA()
    {
        if (teamAScore < maxPlayersPerTeam)
        {
            teamAScore++;
            PlayerPrefs.SetString("PlayerTeam", "TeamA");
            CheckTeams();
        }
    }

    public void JoinTeamB()
    {
        if (teamBScore < maxPlayersPerTeam)
        {
            teamBScore++;
            PlayerPrefs.SetString("PlayerTeam", "TeamB");
            CheckTeams();
        }
    }


    public void CheckTeams()
    {
        if (teamAScore == teamBScore && teamAScore == maxPlayersPerTeam)
        {
            startGameButton.interactable = true; // ��ư Ȱ��ȭ
        }
        else
        {
            startGameButton.interactable = false; // ��ư ��Ȱ��ȭ
        }
    }



    void CreatePlayer()
    {
        // �÷��̾��� ���� �����ɴϴ�.
        PhotonTeam playerTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
        // �� ������ ���ٸ� �Լ� ����
        if (playerTeam == null)
        {
            return;
        }

        // �� �ڵ忡 ���� �ٸ� �������� ����
        string playerPrefab = playerTeam.Name == "TeamA" ? "TeamAPlayer" : "TeamBPlayer";
        Transform spawnPoint = GameObject.Find(playerTeam.Name + "SpawnPoint").GetComponent<Transform>();

        // ���� ����Ʈ�� ���ٸ� ����
        if (spawnPoint == null)
        {
            Debug.LogError("��������Ʈ ���� " + playerTeam.Name);
            return;
        }
        if (playerTeam.Name == "TeamA")
        {
            PhotonNetwork.Instantiate("TeamAPlayer", spawnPoint.position, spawnPoint.rotation, 0);
        }
        else if (playerTeam.Name == "TeamB")
        {
            PhotonNetwork.Instantiate("TeamBPlayer", spawnPoint.position, spawnPoint.rotation, 0);
        }
    }


    public void StartGame()
    {
        isPlaying = true;
        MasterRaiseEvent();
        CloseRoom();
        //CreatePlayersForTeams();
        gameCanvas.GetComponent<Canvas>().enabled = false;
        //CreatePlayer(photonManager._actorNumber);

    }

    /// <summary>
    /// ������Ŭ���̾�Ʈ�� �� �̺�Ʈ�� ���� �濡 ����ִ� ��� ����鿡�� ȣ��ɼ� �ֵ��� �ϴ� �ڵ�
    /// </summary>
    void MasterRaiseEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RaiseEvent(GameStartEventCode, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
    }



    void CloseRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }
    public void EndGame()
    {
        // ��Ÿ ���� ���� ó�� ����
        string sceneName = SceneManager.GetActiveScene().name;
        RestartGameNetwork();

    }
    public void RestartGameNetwork()
    {
        if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ�� ��쿡�� �� �ε� ����
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
        }
    }
    void OpenRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    /// <summary>
    /// ������ �ʱ�ȭ �����ش�.
    /// </summary>
    public void GameReset()
    {
        teamAScore = 0;
        teamBScore = 0;
    }

    private void Update()
    {
        TimeFlow();
        CheckCurrentGame();
        OnlyMasterClient();

    }

    private void UpdateInGameUI()
    {
        if (isPlaying)
        {
            aScrollbar.size = teamAScore / _goalScore;
            bScrollbar.size = teamBScore / _goalScore;
        }
    }

    private void OnlyMasterClient()
    {
        startGameButton.interactable = PhotonNetwork.IsMasterClient;
    }

    public void ResetWinningTeam()
    {
        winningTeam = "None";
    }
    /// <summary>
    /// ���� ���� ������ �޼��Ǵ��� üũ�ϴ� �Լ�
    /// </summary>
    public void CheckCurrentGame()
    {
        if (currentTime <= endTime)
        {
            EndTimeJudge();
        } //�ð� �����
        else if (teamAScore >= _goalScore || teamBScore >= _goalScore)
        {
            EndScoreTeam();
        } //��ǥ ���� ���� ��
        else
        {
            return;
        } //����ó��
    }

    /// <summary>
    /// �ð� �帧üũ�� �Լ�
    /// </summary>
    public void TimeFlow()
    {
        if (isPlaying)
        {
            currentTime -= Time.deltaTime;
            time.text = ((int)currentTime).ToString();
        }
    }
    /// <summary>
    /// ���������� �˸��� bool���� �ٲٴ� �Լ�
    /// </summary>
    public void IsPlayingChange()
    {
        //isPlaying = true;
    }


    public void ResetTime()
    {
        currentTime = 0.0f;
    }
    /// <summary>
    /// �ð� ��� üũ��
    /// </summary>
    public void EndByTime()
    {
        if (currentTime <= endTime)
        {
            EndTimeJudge();
        }
    }
    /// <summary>
    /// �ð� ���ѿ� ������ �� A���� �̰����� TeamA, B���� �¸���� TeamB, ���ٸ� Draw�� ���
    /// </summary>
    /// <returns></returns>
    public void EndTimeJudge()
    {
        isPlaying = false;
        if (teamAScore > teamBScore)
        {
            winningTeam = "TeamA";
        }
        else if (teamBScore > teamAScore)
        {
            winningTeam = "TeamB";
        }
        else
        {
            winningTeam = "Draw";
        }
    }
    /// <summary>
    /// ��ǥ������ ������ ���� ���´ٸ� string������ ��ȯ
    /// </summary>
    /// <returns></returns>
    public void EndScoreTeam()
    {
        isPlaying = false;
        if (teamAScore >= _goalScore)
        {
            winningTeam = "TeamA";
        }
        else if (teamBScore >= _goalScore)
        {
            winningTeam = "TeamB";
        }
        else
        {
            winningTeam = "Draw";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(teamAScore);
            stream.SendNext(teamBScore);
        }
        else
        {
            teamAScore = (float)stream.ReceiveNext();
            teamBScore = (float)stream.ReceiveNext();
        }
        UpdateInGameUI();
    }
}