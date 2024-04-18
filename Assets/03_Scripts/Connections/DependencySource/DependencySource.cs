using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencySource : SingletonOfT<DependencySource>
{

    [Header("����")]
    CustomPlayfab _customPlayfab;

    [Header("���� ����")]
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
