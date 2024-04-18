using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 공통사항을 작성해서 Override 할것
/// </summary>
public  class SceneManagerBase : MonoBehaviour
{

    /// <summary>
    /// 할당이 필요한 객체들을 초기화하는 bucket역할
    /// </summary>
    /// <returns>True == 성공 / False == 실패</returns>
    public virtual bool AssignInstances()
    {
        /* 추가설명
        처음 씬을 부를 때, 초기화가 필요할 때 이 함수를 override 할것 (씬마다 필요한 작업물 담는 함수)
        통일된 명칭을 사용하기
        */
        return false;
    }

    /// <summary>
    /// 설정이 필요한 값들을 초기화하는 bucket역할
    /// </summary>
    /// <returns>True == 성공 / False == 실패</returns>
    public virtual bool InitValuesSet()
    {
        /* 추가설명
        처음 씬을 부를 때, 값을 캐싱해야한다면, 이 함수를 override 할것 (씬마다 필요한 작업물 담는 함수)
        통일된 명칭을 사용하기
        */

        return false;
    }

    /// <summary>
    /// 업데이트 되어야할 함수들을 모으는 Bucket 역할 Update함수
    /// </summary>
    public virtual bool UpdatesSyncWhenChanged()
    {
        /* 추가설명
         * 특정 (Late Update/ Fixed Update) 가 아닌 변화된 현재 값을 체크하기 위함
         */

        return false;
    }

    /// <summary>
    /// 업데이트 뒤에 체크 되어야할 함수들을 모으는 Bucket 역할 LateUpdate함수
    /// </summary>
    public virtual bool LateUpdatesSyncWhenChanged()
    {
        /* 추가설명
         * 특정 (Update/ Fixed Update) 가 아닌 변화된 현재 값을 체크하기 위함
         */
        return false;
    }

    /// <summary>
    ///  함수들을 모으는 Bucket 역할 FixedUpdate함수
    /// </summary>
    public virtual bool FixedLateUpdatesSyncWhenChanged()
    {
        /* 추가설명
         * 특정 (Update/ Late Update) 가 아닌 변화된 현재 값을 체크하기 위함
         */

        return false;
    }
}
