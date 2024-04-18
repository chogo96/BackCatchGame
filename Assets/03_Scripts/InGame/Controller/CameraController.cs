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
    //����ȭ �� ���̾��� ��ֹ���
    [SerializeField] private LayerMask _obstacleMask;
    //����Ű�� ĳ������ Ʈ������
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
        //ī�޶��� �Ÿ��� �÷��̾��� �������� �Ÿ� ���� ���
        float _distance = Vector3.Distance(transform.position, _playerTransform.position);
        //ī�޶�� �÷��̾� ���� ����
        Vector3 _direction = (_playerTransform.position - transform.position).normalized;
        //RaycastHit hits;
        RaycastHit[] Targets = Physics.RaycastAll(_playerTransform.position, _direction,_distance, _obstacleMask);
        foreach (RaycastHit Obstacle in Targets)
        {
            _hitObstacleList.Add(Obstacle);
            //Debug.Log(_hitObstacleList.Count);
            _materialAlphaChanger = Obstacle.transform.GetComponent<MaterialAlphaChanger>();
            _materialAlphaChanger.alphaChange = true;

            if (Targets.Length == 0)
            {
                _materialAlphaChanger.alphaChange = false;
                _hitObstacleList.Clear();
                return;
            }
        }
        

        //�� ���̿� �΋H�� ��� ��ֹ� ���̾ ���� �͵���hits�� ����
        //hits = Physics.RaycastAll(transform.position, _direction, _distance, _obstacleMask);

        //for (int i = 0 ; i < hits.Length; ++i)
        //{
        //    //hits �迭
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