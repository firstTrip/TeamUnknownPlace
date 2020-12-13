using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrap : MonoBehaviour, ICallback
{
    #region Struct

    [System.Serializable]
    public struct StructSoundData
    {
        [Header("사운드 키")] public string SoundKey;
        [Header("사운드웨이브 크기")] public float SoundWaveScale;
        [Header("소리 밸류")] public int SoundValue;

        [Header("발생 타이밍&위치")] public StructSoundPoint[] SoundPoint;
    }

    [System.Serializable]
    public struct StructSoundPoint
    {
        public float Timings;
        public Vector3 PositionAdder;
    }

    #endregion


    [Header("Sound Data")] public StructSoundData SoundData;
    [Space]

    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 끝나는 시간")] public float EffectDuration;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;

    [Space]

    [Header("이펙트 끝나면 발동할 오브젝트")] public ICallback Callback;
    
    private float ActionCoolDownNow;
    private bool IsActive = false;
    private bool isUsed = false;
    private Vector3 SoundWaveScale;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        SoundWaveScale = new Vector3(SoundData.SoundWaveScale, SoundData.SoundWaveScale, 1.0f);
        IsActive = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsActive)
        {
            Action();
        }
    }

    private void Action()
    {
        if (isUsed)
        {
            return;
        }

        isUsed = true;

        // 이펙트 발동
        EffectManager.Instance.SetPool(EffectKey, transform.position, EffectScale);

        // 일정 타이밍마다 소리 생성
        for (int i = 0; i < SoundData.SoundPoint.Length; i++)
        {
            int index = i;
            Action act = () => {
                EffectManager.Instance.SetPool("SoundWave", transform.position + SoundData.SoundPoint[index].PositionAdder, SoundWaveScale);
                AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position + SoundData.SoundPoint[index].PositionAdder);
            };

            IEnumerator coroutine = DUSDJUtil.ActionAfterSecondCoroutine(SoundData.SoundPoint[i].Timings, act);
            StartCoroutine(coroutine);
        }

        // 콜백이 있으면, 이펙트 완전히 끝난 다음에 발동
        if (Callback != null)
        {
            IEnumerator coroutine = DUSDJUtil.ActionAfterSecondCoroutine(EffectDuration, Callback.CallbackAction);
            StartCoroutine(coroutine);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsActive = false;
        }
    }


    #region ICallback

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void CallbackAction()
    {
        Action();
    }

    #endregion

}
