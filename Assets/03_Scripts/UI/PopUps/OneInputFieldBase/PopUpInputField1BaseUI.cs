using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ϳ��� �Է��� ������ �����̵��� �⺻ ���� Ŭ����
/// </summary>
public class PopUpInputField1BaseUI : PopUpBaseUI
{
    /* �߰�����
     * �Ļ� Ŭ������ �ν����� â���� �����ϴ� �������� ����
    */

    #region �Է�â 1���� ���� ������Ƽ
    public string inputField { get => _inputField.text; set { } }
    #endregion

    #region ���κ��� (protected)

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
         #region ���̵� �˻�
         //�̸��� ���� �˻��׸�
         Regex regexRull = new Regex("@");
         Match match1 = regexRull.Match(idText);

         //�̸��� ������ �ƴ϶��
         if (match1.Success == false)
         {
             return false;
         }

         //���̵�κ��� ����ִٸ�
         string[] vals = regexRull.Split(idText);
         if (vals[0].Length <= 0)
         {
             print($"���̵� �պκ� : {vals[0]}");
         }

         //���ڹ��� �Է� �˻��׸�
         string ourPattern = "^[a-zA-Z0-9]";//���ڳ� ����
         regexRull = new Regex(ourPattern);
         if (regexRull.IsMatch(idText)==false)
         {
             return false;
         }
         #endregion

         #region �н����� �˻�

         //���ڼ� >= 4, ���ڶ� ���ڸ� �Է� ����
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
    /// ���� �Լ�
    /// </summary>

    public override void Resetting()
    {
        _inputField.text = null;
    }
}
