using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 닉네임 설정 클래스
/// </summary>
public class PopUpNicknameUI: PopUpInputField1BaseUI
{
    public void OnClickButton_Confirm()
    {
        CavasHide();
    }

    [SerializeField]
    private PopUpRegisterUI _registerUI;


    /// <summary>
    /// 버튼 AddListener
    /// </summary>
    public void OnClickButton_Nickname()
    {
        PopUpLogUI.Instance.logText.text = "닉네임 등록중";
        CustomPlayfab.Instance.TryInputNickname(_inputField.text);
        //Resetting();
    }

    public void OnClickButton_Nick()
    {
        _registerUI.OnClickButton_InputNickname();
    }
}
