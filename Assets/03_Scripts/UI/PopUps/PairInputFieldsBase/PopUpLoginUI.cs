using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PopUpLoginUI : PopUpBaseUI
{
    [SerializeField]
    private TMP_InputField _idInputField;//아이디 입력
    [SerializeField]
    private TMP_InputField _pwInputField;//비밀번호 입력

    /// <summary>
    /// 로그인 버튼
    /// </summary>
    [SerializeField]
    private Button _loginButton;

    /// <summary>
    /// 회원가입 버튼
    /// </summary>
    [SerializeField]
    private Button _RegisterButton;
    [SerializeField]
    private PopUpRegisterUI _registerUI;


    /// <summary>
    /// 버튼 AddListener
    /// </summary>
    public void OnClickButton_Login()
    {
        PopUpLogUI.Instance.logText.text = "로그인 시도중";
        CustomPlayfab.Instance.TryLogin(_idInputField.text, _pwInputField.text);
        //Resetting();
    }

    public void OnClickButton_Register()
    {
        _registerUI.CanvasShow();
    }


}

