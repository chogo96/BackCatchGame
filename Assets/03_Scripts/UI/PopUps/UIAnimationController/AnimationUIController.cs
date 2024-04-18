using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum UIAnimate
{ 
    None,
    Blink,
    MoveToward,
    Shake,
}

/// <summary>
/// 유아이 애니메이션 연출 컨트롤러
/// </summary>
public  class AnimationUIController
{
    BlinkUIAnimation blinkAnim;

    public Canvas logCanvas;

    /// <summary>
    /// isNeedAnimate가 true인 PopUI는 컴포넌트를 붙인다.
    /// </summary>
    public void Need(bool isNeed, UIAnimate anim)
    {
        int index = (int)anim;

        if (isNeed == false)
        {
            return;
        }

        switch (anim)
        {
            case UIAnimate.None:
                break;
            case UIAnimate.Blink:
                break;
            case UIAnimate.MoveToward:
                break;
            case UIAnimate.Shake:
                break;
        }
    }
}

