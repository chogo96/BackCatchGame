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
    /// 텍스트 & 이미지의 한쌍 애니메이션 알파값 컬러의 페이드 아웃 한번
    /// </summary>
    /// <param name="popUpUI">PopUp 상속받은 클래스</param>
    /// <param name="text">사용텍스트</param>
    /// <param name="image">사용이미지</param>
    /// <returns></returns>
    IEnumerator LogPairFadeOut_enumerator(PopUpBaseUI popUpUI, TMP_Text text, Image image = null) 
    {
        //셋팅
        _alpha = 255;
        _useText.color = new Vector4(text.color.r, text.color.g, text.color.b, _alpha);
        _image.color = new Vector4(image.color.r, image.color.g, image.color.b, 87);//

        while (true)
        {
            _alpha = Mathf.Lerp(text.color.a, 0, 2f * Time.deltaTime);

            text.color = new Vector4(text.color.r, text.color.g, text.color.b, _alpha);
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, _alpha);

            if (_alpha <= 0)//끝나면 비활성화
            {
                popUpUI.canvas.enabled = false;
                break;
            }
            yield return null;
        }
    }
}
