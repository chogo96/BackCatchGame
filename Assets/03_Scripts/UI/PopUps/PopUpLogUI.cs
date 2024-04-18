using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 로그찍기용 캔버스 : 등록 필요 없음, 단지 최상단에 노출될 것
/// </summary>
public class PopUpLogUI : PopUpBaseUI
{
    #region 싱글톤 (따로 제작)
    public static PopUpLogUI Instance
    {
        get
        {
            return _instance;
        }
    }
    private static PopUpLogUI _instance;
    public GameObject obj;

    protected override void Awake()
    {
        base.Awake();
        //_myCanvas.enabled = false;

        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(_instance);
            }
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);

        obj = gameObject;
    }
    #endregion

    public TMP_Text logText { get => _logText; }

    [SerializeField]
    private TMP_Text _logText;


}

