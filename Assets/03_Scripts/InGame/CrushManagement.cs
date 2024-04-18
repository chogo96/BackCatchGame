using MJ.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UIElements;
using static StateManagement;

public class CrushManagement : MonoBehaviour
{
    #region ������Ƽ
    /// <summary>
    /// ������ �ð�
    /// </summary>
    public float respawnTime {  get => _respawnTime; set { } }
    /// <summary>
    /// �ǰ� ������ �ʴ� �ð�
    /// </summary>
    public float invisibleTime { get => _invisibleTime; set { } }

    [SerializeField] private float _invisibleTime;
    [SerializeField] private float _respawnTime;
    #endregion
    /// <summary>
    /// �ĸ� ���� ����
    /// </summary>
    [SerializeField][Range(0f, 360f)]private float _backAngle;
    /// <summary>
    /// �ĸ� ������ �Ÿ�
    /// </summary>
    [SerializeField]private float _distance;
    /// <summary>
    /// ���� ���̾ Ž���� ���� ���̾�
    /// </summary>
    [SerializeField]private LayerMask _enemyTeamMask;
   /// <summary>
   /// ���� ���̿� ��, ��������, ��ֹ� ���̾� üũ��
   /// </summary>
    [SerializeField]private LayerMask _obstacleMask;
    /// <summary>
    /// �Ĺ濡 ������ ���� �÷��̾ ���� �����ϰ� �� ���� üũ��
    /// </summary>
    [SerializeField]private bool _vulnerable = false;
    /// <summary>
    /// ����ĳ��Ʈ�� �浹�� �浹ü���� List�� ����
    /// </summary>
    List<Collider> _hitTargetList = new List<Collider>();
    /// <summary>
    /// ���� ������ ��Ÿ�
    /// </summary>
    [SerializeField]private float _attackDistance;
    /// <summary>
    /// �÷��̾� ��ǥ�� ��Ȯ�� �ϱ� ���� ����������
    /// </summary>
    [SerializeField]private float _height;

    /// <summary>
    /// �÷��̾��� Ʈ������
    /// </summary>
    private Transform _playerTransform;
    /// <summary>
    /// �浹ü�� �÷��̾���Ʈ�ѷ�
    /// </summary>
    private PlayerController _enemyPlayerController;
    /// <summary>
    /// �÷��̾��� �÷��̾���Ʈ�ѷ� ��ũ��Ʈ
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
        //������Ʈ�� y���� �������� ȸ���� ������ ��ȯ, ���⿡ 180���� ���� ���� ������Ʈ�� �ٶ󺸴� �ݴ� ������ ������ ���մϴ�.
        float backAngle = (transform.eulerAngles.y) + 180;
        Vector3 rightDir = AngleToDir(backAngle + _backAngle * 0.5f);
        Vector3 leftDir = AngleToDir(backAngle - _backAngle * 0.5f);
        Vector3 lookDir = AngleToDir(backAngle);

        //����Ʈ ������ ���� ����
        _hitTargetList.Clear();

        Collider[] Targets = Physics.OverlapSphere(myPosition, _distance, _enemyTeamMask);

        if (Targets.Length == 0)
        {
            _vulnerable = false;
            return;
        }

        foreach (Collider Enemy in Targets)
        {
            //����ĳ��Ʈ�� ���� �浹ü�� ��ġ�� üũ
            Vector3 targetPos = Enemy.transform.position + transform.up * _height;
            //�浹�� �Ÿ� üũ
            Vector3 targetDir = (targetPos - myPosition).normalized;
            //�浹ü�� ������ ���� ���
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

            if (targetAngle <= _backAngle * 0.5f && !Physics.Raycast(myPosition, targetDir, _distance, _obstacleMask))
            {
                //�׾��� ���� ���� �ȵ�
                if (_playerController.currentState == State.Death || _hitTargetList.Contains(Enemy)) return;

                //����Ʈ�� �浹ü ���� �߰�
                _hitTargetList.Add(Enemy);
                _enemyPlayerController = Enemy.GetComponent<PlayerController>();

                //����� �� �׽�Ʈ������ ����ϴ� �� > TODO �������
                Debug.DrawLine(myPosition, targetPos, Color.red);
                _vulnerable = true;
                if (_vulnerable && _hitTargetList.Contains(Enemy) && Physics.Raycast(myPosition, targetDir, _attackDistance, _enemyTeamMask))
                {
                    _enemyPlayerController.canAttack = true;
                    _playerController.IsDamaged();
                    //�׾��ų� �������� ���� ���� �ߺ� ������ üũ ���� ���� ������ �����ΰ�

                    if (_playerController.hpCount >= 2)
                    {
                        _playerController.isLive = false;
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// �� OnDrawGizmos�� �����뵵�� ���˴ϴ�. ���� Scene���� ����˴ϴ�.
    /// </summary>
    private void OnDrawGizmos()
    {
        //�� ������ üũ ������Ʈ ��ġ���� ���� �� ���� ������ ������ >�����ϰ� ����� ���� ����� (�����ʿ�)
        Vector3 myPosition = transform.position + transform.up * _height;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myPosition, _distance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(myPosition, _attackDistance);

        //������Ʈ�� y���� �������� ȸ���� ������ ��ȯ, ���⿡ 180���� ���� ���� ������Ʈ�� �ٶ󺸴� �ݴ� ������ ������ ���մϴ�.
        float backAngle = (transform.eulerAngles.y)+ 180;  
        Vector3 rightDir = AngleToDir(backAngle + _backAngle * 0.5f);
        Vector3 leftDir = AngleToDir(backAngle - _backAngle * 0.5f);
        Vector3 lookDir = AngleToDir(backAngle);

        Debug.DrawRay(myPosition, rightDir * _distance, Color.black);
        Debug.DrawRay(myPosition, leftDir * _distance, Color.black);
        Debug.DrawRay(myPosition, lookDir * _distance, Color.yellow);
    }
    //������ ���Ͱ����� �ٲ��ִ� �Լ�
        Vector3 AngleToDir(float angle)
    {
        //������ �������� ��ȯ�ϰ�.
        float radian = angle * Mathf.Deg2Rad;
        //������ �ش��ϴ� ���� ���� ��� , ���� ���� ����, 0 , ���� ���� �ڻ������� �����. > ���⺤�ʹ� xz��鿡�� �����ϱ⿡ y�� ��ǥ�� 0���� ó��
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

}
