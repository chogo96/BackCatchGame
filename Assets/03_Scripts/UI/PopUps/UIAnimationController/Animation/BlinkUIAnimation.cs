using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkUIAnimation : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _useText;

    [SerializeField]
    private Image _image;

    float _alpha;

    public void DoStartCorution(IEnumerator Repeat)
    {
        StartCoroutine(Repeat);
    }

    /// <summary>
    /// �ؽ�Ʈ & �̹����� �ѽ� �ִϸ��̼� ���İ� �÷��� ���̵� �ƿ� �ѹ�
    /// </summary>
    /// <param name="popUpUI">PopUp ��ӹ��� Ŭ����</param>
    /// <param name="text">����ؽ�Ʈ</param>
    /// <param name="image">����̹���</param>
    /// <returns></returns>
    IEnumerator LogPairFadeOut_enumerator(PopUpBaseUI popUpUI, TMP_Text text, Image image = null) 
    {
        //����
        _alpha = 255;
        _useText.color = new Vector4(text.color.r, text.color.g, text.color.b, _alpha);
        _image.color = new Vector4(image.color.r, image.color.g, image.color.b, 87);//

        while (true)
        {
            _alpha = Mathf.Lerp(text.color.a, 0, 2f * Time.deltaTime);

            text.color = new Vector4(text.color.r, text.color.g, text.color.b, _alpha);
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, _alpha);

            if (_alpha <= 0)//������ ��Ȱ��ȭ
            {
                popUpUI.canvas.enabled = false;
                break;
            }
            yield return null;
        }
    }
}
