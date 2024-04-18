using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 유아이 기능에 대한 구성 틀, 기본 UI 추상클래스
/// </summary>
public abstract class BaseUI<T> : MonoBehaviour

{
    #region 프로퍼티
    /// <summary>
    /// 현재 소팅순서
    /// </summary>
    public abstract int sortOrder
    {
        get; set;
    }

    /// <summary>
    /// 캔버스 단위
    /// </summary>
    public abstract Canvas canvas
    {
        get; set;
    }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    public abstract bool isEnable 
    {
        get; set;
    }

    /// <summary>
    /// 켜졌을 때, 실행할 콜백
    /// </summary>
    public virtual Action<T> on
    {
        get; set;
    }

    /// <summary>
    /// 꺼졌을 때, 실행할 콜백
    /// </summary>
    public virtual Action<T> off
    {
        get; set;
    }

    public virtual bool isNeedAnimate
    {
        get; set;
    }
    #endregion

    public abstract void Resetting();
    
    public abstract void CanvasShow();
    public abstract void CavasHide();
    public abstract void Switching(bool isTrue);

}
