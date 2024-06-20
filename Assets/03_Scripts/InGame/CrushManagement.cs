using MJ.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static StateManagement;

public class CrushManagement : MonoBehaviour
{

    /// <summary>
    /// 후면 판정 각도
    /// </summary>
    [SerializeField][Range(0f, 360f)]private float _backAngle;
    /// <summary>
    /// 후면 판정할 거리
    /// </summary>
    [SerializeField]private float _distance;
    /// <summary>
    /// 적의 레이어를 탐지할 적팀 레이어
    /// </summary>
    [SerializeField]private LayerMask _enemyTeamMask;
   /// <summary>
   /// 적과 사이에 벽, 지형지물, 장애물 레이어 체크용
   /// </summary>
    [SerializeField]private LayerMask _obstacleMask;
    /// <summary>
    /// 후방에 접근해 적이 플레이어를 공격 가능하게 된 상태 체크용
    /// </summary>
    [SerializeField]private bool _vulnerable = false;
    /// <summary>
    /// 레이캐스트에 충돌한 충돌체들을 List로 보관
    /// </summary>
    List<Collider> _hitTargetList = new List<Collider>();
    /// <summary>
    /// 공격 가능한 사거리
    /// </summary>
    [SerializeField]private float _attackDistance;
    /// <summary>
    /// 플레이어 좌표를 정확히 하기 위한 오차보정용
    /// </summary>
    [SerializeField]private float _height;

    /// <summary>
    /// 플레이어의 트랜스폼
    /// </summary>
    private Transform _playerTransform;
    /// <summary>
    /// 충돌체의 플레이어컨트롤러
    /// </summary>
    private PlayerController _enemyPlayerController;
    /// <summary>
    /// 플레이어의 플레이어컨트롤러 스크립트
    /// </summary>
    private PlayerController _playerController;




    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerTransform = GetComponent<Transform>();
    }
    private void Update()
    {
        Vector3 myPosition = transform.position + transform.up * _height;
        //오브젝트가 y축을 기준으로 회전한 각도를 반환, 여기에 180도를 더해 현재 오브젝트가 바라보는 반대 방향의 각도를 구합니다.
        float backAngle = (transform.eulerAngles.y) + 180;
        Vector3 rightDir = AngleToDir(backAngle + _backAngle * 0.5f);
        Vector3 leftDir = AngleToDir(backAngle - _backAngle * 0.5f);
        Vector3 lookDir = AngleToDir(backAngle);

        //리스트 내용을 전부 제거
        _hitTargetList.Clear();
        if (!GameManager.Instance.isPlaying) return;

        Collider[] Targets = Physics.OverlapSphere(myPosition, _distance, _enemyTeamMask);


        if (Targets.Length == 0)
        {
            _vulnerable = false;
            return;
        }
        
        foreach (Collider Enemy in Targets)
        {
            //레이캐스트에 들어온 충돌체의 위치값 체크
            Vector3 targetPos = Enemy.transform.position + transform.up * _height;
            //충돌한 거리 체크
            Vector3 targetDir = (targetPos - myPosition).normalized;
            //충돌체와 나와의 각도 계산
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

            if (targetAngle <= _backAngle * 0.5f && !Physics.Raycast(myPosition, targetDir, _distance, _obstacleMask))
            {
                //죽었을 때는 판정 안됨
                if (_playerController.currentState == State.Death || _hitTargetList.Contains(Enemy)) return;

                //리스트에 충돌체 정보 추가
                _hitTargetList.Add(Enemy);
                _enemyPlayerController = Enemy.GetComponent<PlayerController>();

                //디버깅 및 테스트용으로 사용하는 줄 > TODO 삭제요망
                Debug.DrawLine(myPosition, targetPos, Color.red);
                _vulnerable = true;
                if (_vulnerable && _hitTargetList.Contains(Enemy) && Physics.Raycast(myPosition, targetDir, _attackDistance, _enemyTeamMask) && !_enemyPlayerController._damaged)
                {
                    _enemyPlayerController.IsAttack();
                    _playerController.IsDamaged();
                    //죽었거나 데미지를 입을 때는 중복 데미지 체크 적이 공격 가능한 상태인가

                    if (_playerController.hpCount >= 2)
                    {
                        _playerController.isLive = false;
                        _playerController.scoreUpCheck = true;
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 이 OnDrawGizmos는 디버깅용도로 사용됩니다. 오직 Scene에만 적용됩니다.
    /// </summary>
    private void OnDrawGizmos()
    {
        //내 포지션 체크 오브젝트 위치보다 조금 더 높게 설정한 이유는 >점프하고 닿았을 때를 대비함 (수정필요)
        Vector3 myPosition = transform.position + transform.up * _height;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myPosition, _distance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(myPosition, _attackDistance);

        //오브젝트가 y축을 기준으로 회전한 각도를 반환, 여기에 180도를 더해 현재 오브젝트가 바라보는 반대 방향의 각도를 구합니다.
        float backAngle = (transform.eulerAngles.y)+ 180;  
        Vector3 rightDir = AngleToDir(backAngle + _backAngle * 0.5f);
        Vector3 leftDir = AngleToDir(backAngle - _backAngle * 0.5f);
        Vector3 lookDir = AngleToDir(backAngle);

        Debug.DrawRay(myPosition, rightDir * _distance, Color.black);
        Debug.DrawRay(myPosition, leftDir * _distance, Color.black);
        Debug.DrawRay(myPosition, lookDir * _distance, Color.yellow);
    }
    //각도를 벡터값으로 바꿔주는 함수
    Vector3 AngleToDir(float angle)
    {
        //각도를 라디안으로 반환하고.
        float radian = angle * Mathf.Deg2Rad;
        //각도에 해당하는 방향 벡터 계산 , 라디안 값의 사인, 0 , 라디안 값의 코사인으로 계산함. > 방향벡터는 xz평면에만 존재하기에 y축 좌표는 0으로 처리
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

}
