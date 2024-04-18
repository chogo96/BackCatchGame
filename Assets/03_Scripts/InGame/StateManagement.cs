using MJ.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// State�� Ʋ�� ���⼭ ����� �ְ� �������� PlayerController���� �����Ұ���
/// </summary>
public class StateManagement : SingletonOfT<StateManagement>
{

    //_state ĳ���� ���¸� enum������ ����
    //�߰��ϰ� ���� ���°� �ִٸ� enum�� �߰� �� case�� State.Case �������� �߰��ϸ� ��
    public enum State
    {
        Idle, Move, Attack, Damage, Death
    }
    private void Awake()
    {
        Init();
        DontDestroyOnLoad(this);
    }
}
