using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 하나의 입력을 가지는 유아이들의 기본 설계 클래스
/// </summary>
public class PopUpInputField1BaseUI : PopUpBaseUI
{
    /* 추가설명
     * 파생 클래스의 인스펙터 창에서 연결하는 형식으로 제작
    */

    #region 입력창 1개에 대한 프로퍼티
    public string inputField { get => _inputField.text; set { } }
    #endregion

    #region 내부변수 (protected)

    [SerializeField]
    protected TMP_InputField _inputField;
  
    [SerializeField]
    protected Button _confirmButton;
    #endregion

    public bool CheckInputFormat()
    {
         if (string.IsNullOrEmpty(_inputField.text) || string.IsNullOrWhiteSpace(_inputField.text))
         {
             return false;
         }

        return true;

         /*
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
         */

    }

    /// <summary>
    /// 리셋 함수
    /// </summary>

    public override void Resetting()
    {
        _inputField.text = null;
    }
}
