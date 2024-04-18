using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonOfT<GameManager>
{
    /// <summary>
    /// ������ ���� �Ǹ� true, ���� ���� ���� �� false
    /// </summary>
    public bool isPlaying;

    [SerializeField]public static int teamAScore;
    [SerializeField]public static int teamBScore;
    
    [SerializeField] private int _goalScore;

    /// <summary>
    /// �¸� ���� �˸��� string��
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
    /// ������ �ʱ�ȭ �����ش�.
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
    /// ���� ���� ������ �޼��Ǵ��� üũ�ϴ� �Լ�
    /// </summary>
    public void CheckCurrentGame()
    {
        if(currentTime == endTime)
        {
            EndTimeJudge();
        } //�ð� �����
        else if (teamAScore >= _goalScore || teamBScore >= _goalScore)
        {
            EndScoreTeam();
        } //��ǥ ���� ���� ��
        else
        {
            return;
        } //����ó��
    }

    /// <summary>
    /// �ð� �帧üũ�� �Լ�
    /// </summary>
    public void TimeFlow()
    {
        currentTime += Time.deltaTime;
    }
    /// <summary>
    /// ���������� �˸��� bool���� �ٲٴ� �Լ�
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
    /// �ð� ��� üũ��
    /// </summary>
    public void EndByTime()
    {
        if (currentTime == endTime)
        {
            EndTimeJudge();
        }
    }
    /// <summary>
    /// �ð� ���ѿ� ������ �� A���� �̰����� TeamA, B���� �¸���� TeamB, ���ٸ� Draw�� ���
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
    /// ��ǥ������ ������ ���� ���´ٸ� string������ ��ȯ
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
