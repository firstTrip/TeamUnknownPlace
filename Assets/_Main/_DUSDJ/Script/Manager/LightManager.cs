using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static LightManager instance;
    public static LightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(LightManager)) as LightManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "LightManager";
                    instance = container.AddComponent(typeof(LightManager)) as LightManager;
                }
            }

            return instance;
        }
    }

    #endregion


    #region Blue Light
    public Light2D BlueLight;
    public float OriginIntensity = 1;
    public float InnerRadius = 12;
    public float OuterRadius = 18;

    IEnumerator BlueLightCoroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CloseBlueLight();
        }
    }

    public void SetBlueLight(Vector3 pos)
    {
        if (BlueLightCoroutine != null)
        {
            StopCoroutine(BlueLightCoroutine);
        }

        BlueLight.transform.position = pos;

        BlueLight.gameObject.SetActive(true);
    }

    public void CloseBlueLight()
    {
        if(BlueLightCoroutine != null)
        {
            StopCoroutine(BlueLightCoroutine);
        }
        BlueLightCoroutine = CloseBlueLightCoroutine();
        StartCoroutine(BlueLightCoroutine);
    }

    IEnumerator CloseBlueLightCoroutine()
    {
        float t = 0;

        while(t < 2.0f)
        {
            t += Time.deltaTime;

            BlueLight.intensity = Mathf.Lerp(OriginIntensity, 0, t / 2.0f);
            BlueLight.pointLightInnerRadius = Mathf.Lerp(InnerRadius, 0, t / 2.0f);
            BlueLight.pointLightOuterRadius = Mathf.Lerp(OuterRadius, 0, t / 2.0f);

            yield return null;
        }

        BlueLight.gameObject.SetActive(false);
    }

    #endregion

    #region Main Light (빛이 1개라는 전제)

    [HideInInspector] public LightMonster MainLight;
    private Transform LightEndPosition;
    private Vector3 offSet;

    #endregion

    #region LightArea & Sound 관련

    private LightArea NowLightArea;

    private Sound NowSound;
    private IEnumerator DurationCoroutine;

    #endregion

    private void Awake()
    {
        #region SingleTone

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        #endregion
    }


    public void Init()
    {
        MainLight = GameObject.Find("MainLight").GetComponent<LightMonster>();
        MainLight.Init();

        BlueLight = GameObject.Find("BlueLight").GetComponent<Light2D>();

        LightEndPosition = MainLight.transform.Find("LightEnd");
        offSet = MainLight.transform.position - LightEndPosition.transform.position;
    }

    // 사실상 위치선정밖에 없는 정도?
    public void SetLightArea(LightArea la)
    {
        if (NowLightArea == la)
        {
            return;
        }
        

        // 기존 LA 요소 전부 끄기
        if (NowLightArea != null)
        {
            NowLightArea.IsEnter = false;
        }
        NowSound = null;
        StopDurationCoroutine();

        // 새 LA 작동
        NowLightArea = la;
        NowLightArea.IsEnter = true;


        if (NowLightArea.LightPosition != null)
        {
            //MainLight.transform.position = NowLightArea.LightPosition.position;
        }
        offSet = MainLight.transform.position - LightEndPosition.transform.position;

        // 일단은 플레이어를 비춘다?
        //MainLight.transform.DOKill();
        //MainLight.transform.DOMove(GameManager.Instance.PlayerChara.transform.position + offSet, 0.2f);
    }


    #region Look By Sound & Duration Coroutine

    public void LookBySound(Sound s)
    {
        // LightArea 확인
        if (NowLightArea == null
            || NowLightArea.gameObject.activeSelf == false)
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
        //MainLight.transform.DOKill();
        //MainLight.transform.DOMove(source + offSet, 2.0f).SetEase(Ease.InQuad);

        SetDuration(duration);
    }

    private void SetDuration(float d)
    {
        StopDurationCoroutine();

        DurationCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(d, () => { NowSound = null; });
        StartCoroutine(DurationCoroutine);
    }

    private void StopDurationCoroutine()
    {
        if (DurationCoroutine != null)
        {
            StopCoroutine(DurationCoroutine);
            DurationCoroutine = null;
        }
    }

    #endregion


    // 현재 미사용, 회전함수
    private void Look(Transform light, Transform target)
    {
        Vector3 ShootPoint = target.transform.position;

        float AngleRad = Mathf.Atan2(ShootPoint.y - light.position.y, ShootPoint.x - light.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        light.rotation = Quaternion.Euler(0, 0, AngleDeg + 90);

    }
}
