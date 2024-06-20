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
    /// 게임이 시작 되면 true, 게임 점수 도달 시 false
    /// </summary>
    public bool isPlaying;

    public static float teamAScore;
    public static float teamBScore;

    [SerializeField] private float _goalScore;

    /// <summary>
    /// 승리 팀을 알리는 string값
    /// </summary>
    public string winningTeam;
    public float endTime;
    public float currentTime;

    public int maxPlayersPerTeam = 5; // 각 팀의 최대 플레이어 수 설정
    public GameObject gameCanvas;
    public Button startGameButton; // 게임 시작 버튼
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
            startGameButton.interactable = true; // 버튼 활성화
        }
        else
        {
            startGameButton.interactable = false; // 버튼 비활성화
        }
    }



    void CreatePlayer()
    {
        // 플레이어의 팀을 가져옵니다.
        PhotonTeam playerTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
        // 팀 정보가 없다면 함수 종료
        if (playerTeam == null)
        {
            return;
        }

        // 팀 코드에 따라 다른 프리팹을 선택
        string playerPrefab = playerTeam.Name == "TeamA" ? "TeamAPlayer" : "TeamBPlayer";
        Transform spawnPoint = GameObject.Find(playerTeam.Name + "SpawnPoint").GetComponent<Transform>();

        // 스폰 포인트가 없다면 에러
        if (spawnPoint == null)
        {
            Debug.LogError("스폰포인트 없음 " + playerTeam.Name);
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
    /// 마스터클라이언트가 쏜 이벤트가 게임 방에 들어있는 모든 사람들에게 호출될수 있도록 하는 코드
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
        // 기타 게임 종료 처리 로직
        string sceneName = SceneManager.GetActiveScene().name;
        RestartGameNetwork();

    }
    public void RestartGameNetwork()
    {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트인 경우에만 씬 로드 실행
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
    /// 점수를 초기화 시켜준다.
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
    /// 게임 종료 조건이 달성되는지 체크하는 함수
    /// </summary>
    public void CheckCurrentGame()
    {
        if (currentTime <= endTime)
        {
            EndTimeJudge();
        } //시간 종료시
        else if (teamAScore >= _goalScore || teamBScore >= _goalScore)
        {
            EndScoreTeam();
        } //목표 점수 도달 시
        else
        {
            return;
        } //예외처리
    }

    /// <summary>
    /// 시간 흐름체크용 함수
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
    /// 게임중임을 알리는 bool값을 바꾸는 함수
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
    /// 시간 경과 체크함
    /// </summary>
    public void EndByTime()
    {
        if (currentTime <= endTime)
        {
            EndTimeJudge();
        }
    }
    /// <summary>
    /// 시간 제한에 끝났을 때 A팀이 이겼으면 TeamA, B팀의 승리라면 TeamB, 비겼다면 Draw를 출력
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
    /// 목표점수에 도달한 팀이 나온다면 string값으로 변환
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