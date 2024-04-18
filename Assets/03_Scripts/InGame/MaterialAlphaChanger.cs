using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialAlphaChanger : MonoBehaviour
{
    /// <summary>
    /// 반투명화 되는 하는 조건
    /// </summary>
    public bool alphaChange { get => _alphaChange; set { _alphaChange = value; } }
    private bool _alphaChange;



    private void Update()
    {
        ChangeAlpha();
    }
    /// <summary>
    /// 충돌한 장애물의 머테리얼의 알파값을 조절하는 기능 함수
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
            }//장애물이 조건에서 벗어났을 때

            _materialColor.a = 0.5f;

            _material.color = _materialColor;
        }//장애물이 존재하는 지 체크하기 위한 조건문
    }
}
