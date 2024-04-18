using System;
using System.Collections.Generic;
using UnityEngine;


public class PopUpUIManager : MonoBehaviour
{
   
    /// <summary>
    /// Stack구조로 현재 띄어진 UI 받기위함
    /// </summary>
    public Stack<PopUpBaseUI> uis = new Stack<PopUpBaseUI>();
    [SerializeField]
    public List<PopUpBaseUI> uiList =new List<PopUpBaseUI> ();//인스펙터에서 보기 위함

    public void Awake()
    {
        Resetting();
    }
    public void Resetting()
    {
        uiList.Clear();

        for (int i = 0; i < uis.Count; i++)
        {
            //배열 -> 대입
            uiList.Add(uis.ToArray()[i]);
            print(uiList[i].gameObject);
        }
    }


    /// <summary>
    /// 오더레이더 순서에 변화가 생기면 호출하여 canvas의 SortingOrder를 수정할 수 있는 함수
    /// </summary>
    /// <param name="canvas"></param>
    public void ReSortingOrder(Canvas canvas)
    {
        int sortingNum = -1;
        if (uis.Count <= 0)
        {
            sortingNum = 0;

        }
        else
        { 
            sortingNum = uis.Count;//최상단 소팅오더
            print(uis.Peek().gameObject.name + $" 내가 선택한 : {sortingNum}");
        }
        canvas.sortingOrder = sortingNum;

        PopUpLogUI.Instance.canvas.sortingOrder = sortingNum + 1;
    }

    /// <summary>
    /// 무엇이 팝업이 되건 마지막에 보여줘야하는 유아이들을 차례로 추가할 함수
    /// </summary>
    /// <returns></returns>
    private void DicideToShowLastestUIs(int maxCount)
    {
        //

        //로그는 최 상단에 노출
        PopUpLogUI.Instance.canvas.sortingOrder = ++maxCount;
    }
}

