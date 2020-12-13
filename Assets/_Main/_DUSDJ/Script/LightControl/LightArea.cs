using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using DG.Tweening;

public class LightArea : MonoBehaviour
{
    /**
     * 아직 미확정.
     * 통째로 지울수도 있는 테스트 클래스임.
     */


    #region Values

    [Header("MainLight가 자리할 곳")] public Transform LightPosition;

    public bool IsEnter = false;

    #endregion

    private void Awake()
    {
        IsEnter = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 들어가있음
        if (IsEnter)
        {
            return;
        }

        // Enter!
        if (collision.CompareTag("Player"))
        {
            LightManager.Instance.SetLightArea(this);
        }

        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsEnter = false;
        }
    }
}
