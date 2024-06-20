using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Diagnostics;


public class CustomPlayfab : SingletonOfT<CustomPlayfab>
{
    public GetAccountInfoResult accountInfo { get => _accountInfo;  set { } } 
    public bool isloginSuccess { get => _isloginSuccess;  set { } } 

    private bool _isloginSuccess;//로그인 내부적으로 판단하기 위해사용 ( false / true )

    private GetAccountInfoResult _accountInfo = new GetAccountInfoResult();//세부정보, 닉네임이랑 이메일을 알 수 있다

    private void Awake()
    {
        Init();
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        //타이틀 아이디 비어있다면
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "F651A";
            PopUpLogUI.Instance.logText.text = "에러발생, playfabs TitleID 세팅이 필요합니다";
        }
    }

#region 로그인 회원가입
    /// <summary>
    /// 플래이펩 로그인 시도 함수
    /// </summary>
    /// <param name="id">입력된 아이디</param>
    /// <param name="pw">입력된 비밀번호</param>
    public void TryLogin(string id, string pw)
    {
        var request = new LoginWithEmailAddressRequest { Email = id , Password = pw};//입력값 기준으로
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);//로그인 시도
    }

    /// <summary>
    /// 플래이펩 회원가입 시도 함수
    /// </summary>
    /// <param name="id">입력된 아이디</param>
    /// <param name="pw">입력된 비밀번호</param>
    public void TryRegister(string id, string pw)
    { 
        //RequireBothUsernameAndEmail 디폴트: true라서, 설정해야 닉네임 입력없이 등록 가능하다
        var request = new RegisterPlayFabUserRequest { Email = id , Password = pw ,RequireBothUsernameAndEmail = false};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }


    /// <summary>
    /// 로그인 성공시 콜백으로 실행되는 함수
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result) 
    {
        _isloginSuccess = true;
        //print(_loginResult.LastLoginTime);
        //print(_result.NewlyCreated);//모름
        //print(_result.PlayFabId);//플래이펩 계정
        //print(_result.CustomData);//모름
        //print(_result.EntityToken.Entity.Id);//플래이펩 계정
        //print(_result?.EntityToken.Entity.Type);//플래이펩 타입

        PopUpLogUI.Instance.logText.text = "로그인 성공";
        CustomPhoton.Instance.JoinLobby();//성공하면 바로 로비 보냄

        GetInformation();
    }

    /// <summary>
    /// 로그인 실패시 콜백으로 실행되는 함수
    /// </summary>
    /// <param name="error"></param>
    private void OnLoginFailure(PlayFabError error)
    {
        PopUpLogUI.Instance.logText.text = $"로그인 실패";
        PopUpInformWindowsUI.Instance.ERROR_Inform("로그인 되지 않았습니다", "아이디 또는 비밀번호를 확인 후, 다시 입력해주시기 바랍니다");
    }

    /// <summary>
    /// 회원가입 성공시 콜백으로 실행되는 함수
    /// </summary>
    /// <param name="result"></param>
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        PopUpLogUI.Instance.logText.text = "회원가입 성공";
        PopUpInformWindowsUI.Instance.Success_Inform("성공적으로 생성되었습니다", "로그인을 통해 게임을 접속할 수 있습니다");
    }

    /// <summary>
    /// 회원가입 실패시 콜백으로 실행되는 함수
    /// </summary>
    /// <param name="error"></param>
    private void OnRegisterFailure(PlayFabError error)
    {
        print(error);
        PopUpLogUI.Instance.logText.text = "회원가입 실패";
        PopUpInformWindowsUI.Instance.ERROR_Inform("생성되지 않았습니다", "아이디 또는 비밀번호를 확인 후, 다시 입력해주시기 바랍니다");
    }
    #endregion

    #region 닉네임등록
    public void TryInputNickname(string nickname)
    {
        var request = new RegisterPlayFabUserRequest { Username = nickname };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnInputNicknameSuccess, OnInputNicknameFail);
    }
    private void OnInputNicknameSuccess(RegisterPlayFabUserResult result)
    {
        PopUpLogUI.Instance.logText.text = "닉네임 등록 성공";
        PopUpInformWindowsUI.Instance.Success_Inform("성공!", "닉네임이 등록되었습니다.");
    }
    private void OnInputNicknameFail(PlayFabError error)
    {
        print(error);
        PopUpLogUI.Instance.logText.text = "닉네임 등록실패";
        PopUpInformWindowsUI.Instance.ERROR_Inform("생성되지 않았습니다", "다시 한번 입력해주세요. 인터넷 혹은 서버점검일 수 있습니다.");
    }
    #endregion

    private void Update()
    {

    }

    /// <summary>
    /// 유저 데이터를 가져오는 함수
    /// </summary>
    void GetInformation()
    {
        var tryAccountInfo = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(tryAccountInfo, GetUserDataOnSuccess, GetUserDataOnFailure);
    }

    /// <summary>
    /// 유저 데이터를 가져오는데 성공시의 콜백 함수
    /// </summary>
    public void GetUserDataOnSuccess(GetAccountInfoResult result)
    {
        if (_isloginSuccess == false)
        {
            return;
        }
        _accountInfo = result;

        // Todo: 아래에서 읽은 계정 정보에서
        // 닉네임 값이 빈 문자열이면, 닉네임 등록.
        // 빈 문자열이 아니면, 로비 캔버스 보여주기.

        // 아래와 같이 빈 문자열 값인지를 비교할 수 있음.
        // 아래 if 구문은 오류 -> 닉네임 등록하도록. else 일 때 로비 캔버스 보여주기.
        print(_accountInfo.AccountInfo.TitleInfo.DisplayName);//인 게임의 닉네임

        // 닉네임 문자열 값이 비었는지 확인.
        if (string.IsNullOrEmpty(_accountInfo.AccountInfo.TitleInfo.DisplayName) 
            || string.IsNullOrWhiteSpace(_accountInfo.AccountInfo.TitleInfo.DisplayName))
        {
            GameObject.Find("Nickname_Canvas").GetComponent<Canvas>().enabled = true;
        }
        // 안 비었음.
        else
        {
            //로비 캔버스를 보여준다
        }
        
        print(_accountInfo.AccountInfo.PrivateInfo.Email);//유저의 이메일
        print(_accountInfo.AccountInfo.TitleInfo.LastLogin);//유저의 마지막 접속일
    }

    /// <summary>
    /// 유저 데이터를 가져오는데 실패 시의 콜백 함수
    /// </summary>
    public void GetUserDataOnFailure(PlayFabError result)
    { 
        
    }
}

/* 공부중
print(_result.LastLoginTime);
//print(_result.NewlyCreated);//모름
print(_result.PlayFabId);//플래이펩 계정
//print(_result.CustomData);//모름
//print(_result.EntityToken.Entity.Id);//플래이펩 계정
//print(_result?.EntityToken.Entity.Type);//플래이펩 타입

//print(_accountInfo.AccountInfo.TitleInfo.TitlePlayerAccount.Id);//플래이펩에서의 아이디
//print(_accountInfo.AccountInfo.TitleInfo.TitlePlayerAccount.Type);//플래이펩에서의 타입
//print(_accountInfo.AccountInfo.Username);//계정 상의 닉네임
print(_accountInfo.AccountInfo.TitleInfo.DisplayName);//인 게임의 닉네임
print(_accountInfo.AccountInfo.PrivateInfo.Email);//유저의 이메일
*/