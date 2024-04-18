using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글톤 함수 ,
/// <typeparam name="T"> 적용하는 클래스 자료형 </typeparam>
public class SingletonOfT<T> : MonoBehaviour
    where T : SingletonOfT<T>
{
    /// <summary>
    /// 인스턴스 생성 프로퍼티
    /// </summary>
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    private static T _instance;//내부변수

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <returns></returns>
    public virtual bool Init()
    {
        if (_instance == null)//없다면 추가
        {
            _instance = (T)this;
            
        }
        if (_instance != (T)this)
        {
            Destroy(this.gameObject);//삭제
        }
        return true;
    }
}
