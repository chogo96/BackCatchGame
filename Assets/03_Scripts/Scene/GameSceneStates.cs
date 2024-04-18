using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 작성중인 씬 목록들
/// </summary>
public enum EnumScene
{
    Title,
    Lobby,
    Room,
    Game
}
/// <summary>
/// 씬전환을 위한 체크용 클래스
/// </summary>
public class GameSceneStates : SingletonOfT<GameSceneStates>
{
    /* 추가설명
     * int 현재 씬에 해당하는 인덱스의 값에 따라 Enum 상태 체크하여, 씬을 로드하는 방식
     * Todo :설계에 대해 더 고민해야한다.
     */

    #region 프로퍼티
    /// <summary>
    /// 현재 씬
    /// </summary>
    public EnumScene selectScene { get => _selectedScene; set { } }
    #endregion

    #region 내부변수

    /// <summary>
    /// 현 상태를 분기하는 이넘타입 변수
    /// </summary>
    [SerializeField]
    private EnumScene _selectedScene = EnumScene.Title;

    /// <summary>
    /// 현재 씬의 인덱스
    /// </summary>
    [SerializeField]
    private int nowSceneIndex;
    /// <summary>
    /// 이전 씬의 인덱스
    /// </summary>
    [SerializeField]
    private int prevSceneIndex;
    #endregion

    private void Awake()
    {
        if (Init() == true)
        { 
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        //동기화 & 활성화할 스크립트
        nowSceneIndex = NowSceneIndex();
        prevSceneIndex = nowSceneIndex;
    }

    private void Update()
    {
        CheckScene(); 
    }

    /// <summary>
    /// 현재 씬 인덱스 받아오는 함수
    /// </summary>
    /// <returns>현재씬의 Index번호</returns>
    private int NowSceneIndex()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.buildIndex;
    }

    /// <summary>
    /// enum을 체크하여 씬을 조작하는 검사용 함수
    /// </summary>
    private void CheckScene()
    {
        _selectedScene = (EnumScene)nowSceneIndex;
        
        //해당 씬에서 처리 할 것
        switch (_selectedScene)
        {
            case EnumScene.Title:
                
                if (CustomPhoton.Instance.isLogin == false)
                {
                    return;
                }

                GoToNextScene();

                break;
            case EnumScene.Lobby:
                break;
            case EnumScene.Room:
                break;
            case EnumScene.Game:
                break;
        }

        if (prevSceneIndex == nowSceneIndex)
        {
            return;
        }
        //동기화
        prevSceneIndex = nowSceneIndex;
        //GetEnumIndex(_selectedScene);
    }
    /*아직 필요없음
    /// <summary>
    /// enum으로 씬에 대한 숫자를 반환하는 함수
    /// </summary>
    /// <param name="_selectedScene">현재 enum 상태</param>
    /// <returns></returns>
    private int GetEnumIndex(EnumScene _selectedScene)
    {
        return nowSceneIndex = Convert.ToInt32(_selectedScene);
    }
    */
    /// <summary>
    /// EnumScene에 해당하는 sceneindex 통해 씬을 검색하는 기능
    /// </summary>
    private void GetSceneByIndex(int index)
    {
        SceneManager.GetSceneByBuildIndex(index);
    }

    /// <summary>
    /// 씬 부르기
    /// </summary>
    /// <param name="i">씬 인덱스</param>
    public void ChangeScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    /// <summary>
    /// 다음 씬으로 이동하는 함수
    /// </summary>
    public void GoToNextScene()
    {
        /* 추가설명
         * 현재 인덱스를 기준으로 단순히 인덱스를 증가시켜, 다음 씬을 불러온다.
         */

        nowSceneIndex = NowSceneIndex();

        if (nowSceneIndex == SceneManager.sceneCountInBuildSettings -1)
        {
            print("가장 마지막 인덱스의 씬입니다 다음 씬을 불러올 수 없습니다");
            return;
        }

        _selectedScene = (EnumScene)(++nowSceneIndex);
        ChangeScene(nowSceneIndex);
    }

    /// <summary>
    /// 이전 씬으로 이동하는 함수
    /// </summary>
    public void GoToPrevScene()
    {
        /* 추가설명
         * 현재 인덱스를 기준으로 단순히 인덱스를 감소시켜, 이전 씬을 불러온다.
         */

        nowSceneIndex = NowSceneIndex();

        if (nowSceneIndex == 0)
        {
            print("제일 처음 씬입니다, 이전 씬을 불러올 수 없습니다.");
            return;
        }

        _selectedScene = (EnumScene)(--nowSceneIndex);
        ChangeScene(nowSceneIndex);
    }
}
