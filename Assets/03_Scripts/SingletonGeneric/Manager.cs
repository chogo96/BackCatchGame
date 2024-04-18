using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 싱글톤이 적용된 메니저
/// </summary>
public class Manager : SingletonOfT<Manager>
{
    #region 프로퍼티
    public string id { get => _id; set { } }
    public string pw { get => _pw; set { } }
    public PopUpUIManager popUpUIManager { get => _popUpUIManager; }

    #endregion

    #region 연결 목록
    [Header("UI")]
    [SerializeField]
    private PopUpUIManager _popUpUIManager;

    #endregion
    private string _id;
    private string _pw;

    public void Awake()
    {
        if (Init() == true)
        {
            DontDestroyOnLoad(this);
        }
    }

}

