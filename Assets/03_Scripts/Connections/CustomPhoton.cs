using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// 포톤 관련 정리, 싱글톤
/// </summary>
public class CustomPhoton : MonoBehaviourPunCallbacks //프로퍼티와 메소드등 작성되어있다
{
    public static CustomPhoton Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool isLogin { get => _isLogin; set { } }//로그인 조건충족 프로퍼티
    public GameObject roomListContent;
    public GameObject roomListItemPrefab;
    public GameObject teamSelectPanel;

    private static CustomPhoton _instance;
    public GameObject obj;

    bool _isLogin = false;
    public Button createRoomButton;

    /// <summary>
    /// 플레이팹에서 받은 닉네임은 유저 닉네임으로 지정
    /// </summary>
    private string _nickname = CustomPlayfab.Instance.accountInfo.AccountInfo.TitleInfo.DisplayName;
    /// <summary>
    /// 게임의 버전 지금은 프로토 타입으로 지정함
    /// </summary>
    protected void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(_instance);
            }
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);

        obj = gameObject;

        //마스터가 로드레벨시, 나머지 클라이언트가 자동으로 같은 방에 싱크될 수 있도록 제어한다.
        PhotonNetwork.AutomaticallySyncScene = true;
        //접속 유저의 닉네임 설정
        PhotonNetwork.NickName = _nickname;

        //포톤 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    string gameVersion = "1";//클라이언트 버전 체크용 : 출시 후의 변경사항이 없는 한 1로 유지

    void Start()
    {
        Connect();
    }

    #region 서버 연결
    /// <summary>
    /// 연결되었을떄 콜백되는 함수
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PopUpLogUI.Instance.logText.text = "마스터 서버 연결 완료";
    }
    /// <summary>
    /// 연결이 끊어졌을때 콜백되는 함수
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PopUpLogUI.Instance.logText.text = "마스터 서버 연결 실패";
    }

    /// <summary>
    /// 연결하는 함수
    /// </summary>
    public void Connect()
    {
        //서버에 연결되어있지 않으면
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.GameVersion = gameVersion; //현재 버전을 할당한다
            PhotonNetwork.ConnectUsingSettings(); //준비된 구성파일로 서버에 연결하는 함수
            PhotonNetwork.AutomaticallySyncScene = true;
            OnConnectedToMaster();
        }
    }

    #endregion 서버

    #region 로비

    /// <summary>
    /// 타이틀에서 로그인이 성공하여, 로비씬을 넘길 준비가 되었다는 함수
    /// </summary>
    public void JoinLobby()
    {
        if (_isLogin = CustomPlayfab.Instance.isloginSuccess == true)
        {
            PopUpLogUI.Instance.logText.text = "로비 씬 불러오는 중";
            PhotonNetwork.JoinLobby();
            createRoomButton.interactable = true;
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        
    }

    #endregion 로비

    #region 룸

    public void JoinRoom()
    {
        if (_isLogin = CustomPlayfab.Instance.isloginSuccess == true)
        {
            PopUpLogUI.Instance.logText.text = "룸 씬 불러오는 중";
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log($"PhotonInRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }


    }
    #endregion

    /// <summary>
    /// 버튼을 눌렀을 때 팀을 정하는 함수
    /// </summary>
    public void OnTeamSelectButton(int teamID)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "team", teamID } });
    }

    public void OnRoomListUpdated(List<Room> roomList)
    {
        foreach (Transform trans in roomListContent.transform)
        {
            Destroy(trans.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;
            GameObject listItem = Instantiate(roomListItemPrefab, roomListContent.transform);
            listItem.GetComponentInChildren<Text>().text = room.Name;
            listItem.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 방을 닫아 더 이상 플레이어가 참가하지 못하도록 설정
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public void EndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 게임이 끝나면 방을 다시 열어 새로운 플레이어의 입장을 허용
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    public void LoadGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("InGame");
        }
    }
}
