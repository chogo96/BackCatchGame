using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonOfT<GameManager>
{
    /// <summary>
    /// 게임이 시작 되면 true, 게임 점수 도달 시 false
    /// </summary>
    public bool isPlaying;

    [SerializeField]public static int teamAScore;
    [SerializeField]public static int teamBScore;
    
    [SerializeField] private int _goalScore;

    /// <summary>
    /// 승리 팀을 알리는 string값
    /// </summary>
    public string winningTeam;
    public float endTime;
    public float currentTime;
    private void Awake()
    {
        Init();
        GameReset();
        IsPlayingChange();
        ResetWinningTeam();
    }

    /// <summary>
    /// 점수를 초기화 시켜준다.
    /// </summary>
    public void GameReset()
    {
        teamAScore = 0;
        teamBScore = 0;
    }

    private void Update()
    {
        
        TimeFlow();
        CheckCurrentGame();
        Debug.Log(teamAScore);
        Debug.Log(teamBScore);
        if (!isPlaying)
        {

        }

    }

    public void ResetWinningTeam()
    {
        winningTeam = "None";
    }
    /// <summary>
    /// 게임 종료 조건이 달성되는지 체크하는 함수
    /// </summary>
    public void CheckCurrentGame()
    {
        if(currentTime == endTime)
        {
            EndTimeJudge();
        } //시간 종료시
        else if (teamAScore >= _goalScore || teamBScore >= _goalScore)
        {
            EndScoreTeam();
        } //목표 점수 도달 시
        else
        {
            return;
        } //예외처리
    }

    /// <summary>
    /// 시간 흐름체크용 함수
    /// </summary>
    public void TimeFlow()
    {
        currentTime += Time.deltaTime;
    }
    /// <summary>
    /// 게임중임을 알리는 bool값을 바꾸는 함수
    /// </summary>
    public void IsPlayingChange()
    {
        isPlaying = true;
    }

    
    public void ResetTime()
    {
        currentTime = 0.0f;
    }
    /// <summary>
    /// 시간 경과 체크함
    /// </summary>
    public void EndByTime()
    {
        if (currentTime == endTime)
        {
            EndTimeJudge();
        }
    }
    /// <summary>
    /// 시간 제한에 끝났을 때 A팀이 이겼으면 TeamA, B팀의 승리라면 TeamB, 비겼다면 Draw를 출력
    /// </summary>
    /// <returns></returns>
    public void EndTimeJudge()
    {
        isPlaying = false;
        if (teamAScore > teamBScore)
        {
            winningTeam = "TeamA";
        }
        else if (teamBScore > teamAScore)
        {
            winningTeam = "TeamB";
        }
    }
    /// <summary>
    /// 목표점수에 도달한 팀이 나온다면 string값으로 변환
    /// </summary>
    /// <returns></returns>
    public void EndScoreTeam()
    {
        isPlaying = false;
        if (teamAScore >= _goalScore)
        {
            winningTeam = "TeamA";
        }
        else if (teamBScore >= _goalScore)
        {
            winningTeam = "TeamB";
        }
        else 
        { 
            winningTeam = "Draw";
        }
    }
}
