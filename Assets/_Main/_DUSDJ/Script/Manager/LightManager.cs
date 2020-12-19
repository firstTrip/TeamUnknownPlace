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

    public void SetBlueLight(Vector3 pos)
    {
        if (BlueLightCoroutine != null)
        {
            StopCoroutine(BlueLightCoroutine);
        }

        BlueLight.transform.position = pos;

        if (BlueLightCoroutine != null)
        {
            StopCoroutine(BlueLightCoroutine);
        }
        BlueLightCoroutine = SetBlueLightCoroutine();
        StartCoroutine(BlueLightCoroutine);

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

    IEnumerator SetBlueLightCoroutine()
    {
        float t = 0;

        while (t < 2.0f)
        {
            t += Time.deltaTime;

            BlueLight.intensity = Mathf.Lerp(0, OriginIntensity, t / 2.0f);
            BlueLight.pointLightInnerRadius = Mathf.Lerp(0, InnerRadius, t / 2.0f);
            BlueLight.pointLightOuterRadius = Mathf.Lerp(0, OuterRadius, t / 2.0f);

            yield return null;
        }
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
    private InnerLightMovement innerLightMovement;

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

        innerLightMovement = MainLight.GetComponentInChildren<InnerLightMovement>();
        innerLightMovement.StartMovement();

        BlueLight = GameObject.Find("BlueLight").GetComponent<Light2D>();
    }


    #region HearSound, DeadCheck

    public void HearSound(Sound s)
    {
        MainLight.NowSound = s;
    }

    public void DeadCheck(GameObject from)
    {
        MainLight.DeadCheck(from);        
    }

    #endregion

    #region Banish, Reveal

    public void Banish()
    {
        MainLight.Banish();

        innerLightMovement.EndMovement();
    }

    public void Reveal()
    {
        MainLight.Reveal();

        innerLightMovement.StartMovement();
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
