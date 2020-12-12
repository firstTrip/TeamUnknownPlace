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

    public static LightArea instance;


    public Light2D MainLight;
    public Transform LightPosition;

    public Transform LightEndPosition;
    private Vector3 offSet;


    public bool IsEnter = false;

    private Transform target;


    #region Sound 관련

    public Sound NowSound;
    private IEnumerator DurationCoroutine;

    #endregion




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        offSet = MainLight.transform.position - LightEndPosition.transform.position;

        LightManager.Instance.LightCheckSound += LookBySound;
    }

    private void Update()
    {
        if (IsEnter)
        {
            //Look(MainLight.transform, target.transform);


        }
    }

    public void LookBySound(Sound s)
    {
        if (!IsEnter)
        {
            return;
        }

        // 현재 집중하는 소리가 없으면 바로 새 사운드를 향한다.
        if (NowSound == null)
        {
            NowSound = s;
            LookBySound(s.Duration, s.Position);
            return;
        }

        // 작은 소리는 무시한다.
        if (s.Value < NowSound.Value)
        {
            return;
        }

        // 동등 이상의 소리일 때 : 새 사운드에 집중
        NowSound = s;
        LookBySound(s.Duration, s.Position);
    }

    public void LookBySound(float duration, Vector3 source)
    {
        if (!IsEnter)
        {
            return;
        }

        MainLight.transform.DOKill();
        MainLight.transform.DOMove(source + offSet, 0.2f).SetEase(Ease.Linear);

        SetDuration(duration);
    }

    
    private void SetDuration(float d)
    {
        if (DurationCoroutine != null)
        {
            StopCoroutine(DurationCoroutine);
            DurationCoroutine = null;
        }

        DurationCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(d, () => { NowSound = null; });
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnter)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            IsEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsEnter = false;
        }
    }

    private void Look(Transform light, Transform target)
    {
        Vector3 ShootPoint = target.transform.position;

        float AngleRad = Mathf.Atan2(ShootPoint.y - light.position.y, ShootPoint.x - light.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        light.rotation = Quaternion.Euler(0, 0, AngleDeg + 90);

    }

}
