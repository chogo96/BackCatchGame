using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencySource : SingletonOfT<DependencySource>
{

    [Header("연결")]
    CustomPlayfab _customPlayfab;

    [Header("유저 정보")]
    [SerializeField]
    private string _nickname;
    [SerializeField]
    private string _lastLogin;
    [SerializeField]
    private string _userEmail;


    private void Awake()
    {
        _customPlayfab = CustomPlayfab.Instance;
    }

    private void GetData()
    {
        _nickname = _customPlayfab.accountInfo.AccountInfo.TitleInfo.DisplayName;
        _lastLogin = _customPlayfab.accountInfo.AccountInfo.TitleInfo.LastLogin.ToString();
        _userEmail = _customPlayfab.accountInfo.AccountInfo.PrivateInfo.Email;
    }
}
