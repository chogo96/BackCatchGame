using MJ.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    //투명화 할 레이어의 장애물들
    [SerializeField] private LayerMask _obstacleMask;
    //가리키는 캐릭터의 트랜스폼
    [SerializeField] private Transform _playerTransform;
    private MaterialAlphaChanger _materialAlphaChanger;
    List<RaycastHit> _hitObstacleList = new List<RaycastHit>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _playerTransform.position);
    }
    private void Update()
    {
        //카메라의 거리와 플레이어의 포지션의 거리 차이 계산
        //float _distance = Vector3.Distance(transform.position, _playerTransform.position);
        ////카메라와 플레이어 간의 각도
        //Vector3 _direction = (_playerTransform.position - transform.position).normalized;
        ////RaycastHit hits;
        ////RaycastHit[] Targets = Physics.RaycastAll(_playerTransform.position, _direction,_distance, _obstacleMask);
        //foreach (RaycastHit Obstacle in Targets)
        //{
        //    _hitObstacleList.Add(Obstacle);
        //    //Debug.Log(_hitObstacleList.Count);
        //    _materialAlphaChanger = Obstacle.transform.GetComponent<MaterialAlphaChanger>();
        //    _materialAlphaChanger.alphaChange = true;

        //    if (Targets.Length == 0)
        //    {
        //        _materialAlphaChanger.alphaChange = false;
        //        _hitObstacleList.Clear();
        //        return;
        //    }
        //}
        

        //그 사이에 부딫힌 모든 장애물 레이어를 가진 것들을hits에 저장
        //hits = Physics.RaycastAll(transform.position, _direction, _distance, _obstacleMask);

        //for (int i = 0 ; i < hits.Length; ++i)
        //{
        //    //hits 배열
        //    RaycastHit hit = hits[i];

        //    MaterialAlphaChanger materialAlphaChanger = hit.collider.GetComponent<MaterialAlphaChanger>();
        //    if(materialAlphaChanger == null)
        //    {
        //        continue;
        //        //throw new NotImplementedException();
        //    }
        //    materialAlphaChanger.alphaChange = true;

        //}
    }
}