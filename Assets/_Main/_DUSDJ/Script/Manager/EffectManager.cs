using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static EffectManager instance;
    public static EffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(EffectManager)) as EffectManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "EffectManager";
                    instance = container.AddComponent(typeof(EffectManager)) as EffectManager;
                }
            }

            return instance;
        }
    }

    #endregion

    #region Values

    public Dictionary<string, List<Effect>> List;
    public Dictionary<string, Effect> DataDic;

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
        
        /* Data Init */
        List = new Dictionary<string, List<Effect>>();
        DataDic = new Dictionary<string, Effect>();

        Effect[] datas = Resources.LoadAll<Effect>("Effect/");
        for (int i = 0; i < datas.Length; i++)
        {
            string key = datas[i].name;
            DataDic.Add(key, datas[i]);
            List.Add(key, new List<Effect>());
        }
    }

    #region Pooling

    public void IncreasePool(string key, int num)
    {
        Debug.Log(string.Format("IncreasePool({0}) : {1}", num, gameObject.name));

        for (int i = 0; i < num; i++)
        {
            Effect go = Instantiate(DataDic[key]);
            go.transform.SetParent(transform, false);
            List[key].Add(go);
            go.gameObject.SetActive(false);
        }
    }

    public Effect SetPool(string key, Renderer renderer)
    {
        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogWarning(string.Format("없는 이름으로 Effect 생성 : {0}", key));
            return null;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                Effect e = List[key][i];

                if (e.gameObject.activeSelf == false)
                {
                    e.gameObject.SetActive(true);
                    e.SetEffect(renderer);

                    return e;
                }

            }

            IncreasePool(key, 3);
        }
    }

    public void SetPool(string key, Vector3 position)
    {
        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogWarning(string.Format("없는 이름으로 Effect 생성 : {0}", key));
            return;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                Effect e = List[key][i];

                if (e.gameObject.activeSelf == false)
                {
                    e.gameObject.SetActive(true);
                    e.SetEffect(position);

                    return;
                }

            }

            IncreasePool(key, 3);
        }
    }

    public void SetPool(string key, Vector3 position, Vector3 scale)
    {
        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogWarning(string.Format("없는 이름으로 Effect 생성 : {0}", key));
            return;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                Effect e = List[key][i];

                if (e.gameObject.activeSelf == false)
                {
                    e.gameObject.SetActive(true);
                    e.SetEffect(position, scale);

                    return;
                }

            }

            IncreasePool(key, 3);
        }
    }

    #endregion


    #region Camera Effect

    private IEnumerator CameraZoomCoroutine;

    public void ZoomTarget(Transform target, float orthoValue, float duration = -1)
    {
        Transform originFollow = GameManager.Instance.Cinemachine.Follow;
        float originOrthographicSize = GameManager.Instance.Cinemachine.m_Lens.OrthographicSize;

        GameManager.Instance.Cinemachine.Follow = target;
        GameManager.Instance.Cinemachine.m_Lens.OrthographicSize = orthoValue;

        
        if (duration > 0)
        {            
            if(CameraZoomCoroutine != null)
            {
                StopCoroutine(CameraZoomCoroutine);                
            }

            CameraZoomCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(duration, () => {
                ZoomTarget(originFollow, originOrthographicSize);
            });
            StartCoroutine(CameraZoomCoroutine);
        }
    }

    #endregion


}