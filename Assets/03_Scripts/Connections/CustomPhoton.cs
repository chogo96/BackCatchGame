using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    private static CustomPhoton _instance;
    public GameObject obj;

    bool _isLogin = false;

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
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    #endregion 로비
}
