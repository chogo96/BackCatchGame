using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// �ۼ����� �� ��ϵ�
/// </summary>
public enum EnumScene
{
    Title,
    Lobby,
    Room,
    Game
}
/// <summary>
/// ����ȯ�� ���� üũ�� Ŭ����
/// </summary>
public class GameSceneStates : SingletonOfT<GameSceneStates>
{
    /* �߰�����
     * int ���� ���� �ش��ϴ� �ε����� ���� ���� Enum ���� üũ�Ͽ�, ���� �ε��ϴ� ���
     * Todo :���迡 ���� �� ����ؾ��Ѵ�.
     */

    #region ������Ƽ
    /// <summary>
    /// ���� ��
    /// </summary>
    public EnumScene selectScene { get => _selectedScene; set { } }
    #endregion

    #region ���κ���

    /// <summary>
    /// �� ���¸� �б��ϴ� �̳�Ÿ�� ����
    /// </summary>
    [SerializeField]
    private EnumScene _selectedScene = EnumScene.Title;

    /// <summary>
    /// ���� ���� �ε���
    /// </summary>
    [SerializeField]
    private int nowSceneIndex;
    /// <summary>
    /// ���� ���� �ε���
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
        //����ȭ & Ȱ��ȭ�� ��ũ��Ʈ
        nowSceneIndex = NowSceneIndex();
        prevSceneIndex = nowSceneIndex;
    }

    private void Update()
    {
        CheckScene(); 
    }

    /// <summary>
    /// ���� �� �ε��� �޾ƿ��� �Լ�
    /// </summary>
    /// <returns>������� Index��ȣ</returns>
    private int NowSceneIndex()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.buildIndex;
    }

    /// <summary>
    /// enum�� üũ�Ͽ� ���� �����ϴ� �˻�� �Լ�
    /// </summary>
    private void CheckScene()
    {
        _selectedScene = (EnumScene)nowSceneIndex;
        
        //�ش� ������ ó�� �� ��
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
        //����ȭ
        prevSceneIndex = nowSceneIndex;
        //GetEnumIndex(_selectedScene);
    }
    /*���� �ʿ����
    /// <summary>
    /// enum���� ���� ���� ���ڸ� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="_selectedScene">���� enum ����</param>
    /// <returns></returns>
    private int GetEnumIndex(EnumScene _selectedScene)
    {
        return nowSceneIndex = Convert.ToInt32(_selectedScene);
    }
    */
    /// <summary>
    /// EnumScene�� �ش��ϴ� sceneindex ���� ���� �˻��ϴ� ���
    /// </summary>
    private void GetSceneByIndex(int index)
    {
        SceneManager.GetSceneByBuildIndex(index);
    }

    /// <summary>
    /// �� �θ���
    /// </summary>
    /// <param name="i">�� �ε���</param>
    public void ChangeScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    /// <summary>
    /// ���� ������ �̵��ϴ� �Լ�
    /// </summary>
    public void GoToNextScene()
    {
        /* �߰�����
         * ���� �ε����� �������� �ܼ��� �ε����� ��������, ���� ���� �ҷ��´�.
         */

        nowSceneIndex = NowSceneIndex();

        if (nowSceneIndex == SceneManager.sceneCountInBuildSettings -1)
        {
            print("���� ������ �ε����� ���Դϴ� ���� ���� �ҷ��� �� �����ϴ�");
            return;
        }

        _selectedScene = (EnumScene)(++nowSceneIndex);
        ChangeScene(nowSceneIndex);
    }

    /// <summary>
    /// ���� ������ �̵��ϴ� �Լ�
    /// </summary>
    public void GoToPrevScene()
    {
        /* �߰�����
         * ���� �ε����� �������� �ܼ��� �ε����� ���ҽ���, ���� ���� �ҷ��´�.
         */

        nowSceneIndex = NowSceneIndex();

        if (nowSceneIndex == 0)
        {
            print("���� ó�� ���Դϴ�, ���� ���� �ҷ��� �� �����ϴ�.");
            return;
        }

        _selectedScene = (EnumScene)(--nowSceneIndex);
        ChangeScene(nowSceneIndex);
    }
}
