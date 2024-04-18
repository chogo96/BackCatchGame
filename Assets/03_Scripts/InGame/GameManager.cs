using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonOfT<GameManager>
{
    public int TeamAScore;
    public int TeamBScore;
    private void Awake()
    {
        Init();

        TeamAScore = 0;
        TeamBScore = 0;
    }

    private void GameStart()
    {
        TeamAScore = 0;
        TeamBScore = 0;
    }



}
