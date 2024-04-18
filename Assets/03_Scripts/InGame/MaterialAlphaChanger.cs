using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialAlphaChanger : MonoBehaviour
{
    /// <summary>
    /// ������ȭ �Ǵ� �ϴ� ����
    /// </summary>
    public bool alphaChange { get => _alphaChange; set { _alphaChange = value; } }
    private bool _alphaChange;



    private void Update()
    {
        ChangeAlpha();
    }
    /// <summary>
    /// �浹�� ��ֹ��� ���׸����� ���İ��� �����ϴ� ��� �Լ�
    /// </summary>
    private void ChangeAlpha()
    {
        Renderer _obstacleRenderer = transform.GetComponent<Renderer>();

        if (_obstacleRenderer != null) 
        {
            Material _material = _obstacleRenderer.material;
            Color _materialColor = _material.color;
            if (!_alphaChange) 
            {
                _materialColor.a = 1f;
                _material.color = _materialColor;
                return;
            }//��ֹ��� ���ǿ��� ����� ��

            _materialColor.a = 0.5f;

            _material.color = _materialColor;
        }//��ֹ��� �����ϴ� �� üũ�ϱ� ���� ���ǹ�
    }
}
