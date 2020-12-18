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
    }


    #region Hear Sound

    public void HearSound(Sound s)
    {
        MainLight.NowSound = s;
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
