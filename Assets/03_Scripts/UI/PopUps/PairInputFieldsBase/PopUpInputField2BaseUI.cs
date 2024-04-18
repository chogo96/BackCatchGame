using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 두개의 입력을 가지는 유아이들의 기본 설계 클래스
/// </summary>
public class PopUpInputField2BaseUI : PopUpBaseUI
{
    /* 추가설명
    * 파생 클래스의 인스펙터 창에서 연결하는 형식으로 제작
    */

    #region 입력창 2개에 대한 프로퍼티
    public string idInputField { get => _idInputField.text; set { } }
    public string pwInputField { get => _pwInputField.text; set { } }
    #endregion

    #region 내부변수 (protected)
    /*
     * 파생 클래스의 인스펙터 창에서 연결하는 형식으로 제작
     */
    [SerializeField]
    protected TMP_InputField _idInputField;//아이디
    [SerializeField]
    protected TMP_InputField _pwInputField;//비밀번호
    [SerializeField]
    protected Button _confirmButton;
    #endregion

    /// <summary>
    /// 리셋 함수
    /// </summary>
    public override void Resetting()
    {
        _idInputField.text = null;
        _pwInputField.text = null;
    }
    /*
    /// <summary>
    /// 아이디 비밀번호 조건, 입력에 대한 검사 함수
    /// </summary>
    /// <param name="idText"></param>
    /// <param name="pwText"></param>
    /// <returns></returns>
    public bool CheckCondition(string idText, string pwText)
    {
        //빈 공간이라면,
        if (string.IsNullOrEmpty(idText) || string.IsNullOrWhiteSpace(idText))
        {
            return false;
        }
        if (string.IsNullOrEmpty(pwText) || string.IsNullOrWhiteSpace(pwText))
        {
            return false;
        }

        #region 아이디 검사
        //이메일 형식 검사항목
        Regex regexRull = new Regex("@");
        Match match1 = regexRull.Match(idText);

        //이메일 형식이 아니라면
        if (match1.Success == false)
        {
            return false;
        }

        //아이디부분이 비어있다면
        string[] vals = regexRull.Split(idText);
        if (vals[0].Length <= 0)
        {
            print($"아이디 앞부분 : {vals[0]}");
        }

        //숫자문자 입력 검사항목
        string ourPattern = "^[a-zA-Z0-9]";//문자나 숫자
        regexRull = new Regex(ourPattern);
        if (regexRull.IsMatch(idText)==false)
        {
            return false;
        }
        #endregion

        #region 패스워드 검사

        //글자수 >= 4, 숫자랑 문자만 입력 가능
        if (pwText.Length <= 3 || regexRull.IsMatch(pwText)==false)
        {
            return false;
        }

        if (regexRull.IsMatch(pwText) == false)
        {
            return false;
        }
        return true;
        #endregion
    }
    */

}
