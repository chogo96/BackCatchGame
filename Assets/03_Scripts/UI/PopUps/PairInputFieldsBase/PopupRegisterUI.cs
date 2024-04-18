using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 회원가입
/// </summary>
public class PopUpRegisterUI : PopUpInputField2BaseUI
{
    public static PopUpRegisterUI Instance
    {
        get => _instance;
    }
    private static PopUpRegisterUI _instance;


    public override void CanvasShow()
    {
        base.CanvasShow();
        //Init();
    }


    /// <summary>
    /// 등록버튼 클릭시 실행 함수
    /// </summary>
    public void OnClickButton_Register()
    {
        PopUpLogUI.Instance.logText.text = "회원가입 시도중";
        //회원가입 시도
        CustomPlayfab.Instance.TryRegister(_idInputField.text, _pwInputField.text);

        //성공 실패 여부 상관없이 창을 닫는다.
        CavasHide();
        Resetting();
    }

}

