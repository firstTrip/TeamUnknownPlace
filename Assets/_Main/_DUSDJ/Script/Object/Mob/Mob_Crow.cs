﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_Crow : MonoBehaviour, ICallback, IDamagable
{
    [Header("튜토리얼")] public bool Tutorial = true;

    [Header("Sound Data")] public StructSoundData SoundData;
    [Header("루프여부")]public bool SoundLoop = false;

    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 끝나는 시간")] public float EffectDuration;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;

    

    [Space]

    [SerializeField]
    private float hp;
    public float HP
    {
        get { return hp; }
        set
        {
            if(value <= 0)
            {
                
                hp = 0;

                // Dead Effect & Dead
                Dead();

                return;
            }

            hp = value;

            // hp에 따른 변화?
        }
    }

    private bool IsAlive = false;
    private Animator anim;


    #region ICallback

    public void CallbackAction()
    {
        ObjectAction();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        anim = GetComponent<Animator>();

        IsAlive = true;
    }

    public void ObjectAction()
    {
        // 튜토리얼에만 적용
        if (Tutorial)
        {
            // 사냥시작
            LightManager.Instance.MainLight.IsHunting = true;

            // 까마귀 줌 & 플레이어 조작 정지
            EffectManager.Instance.ZoomTarget(transform, 3.0f);
            GameManager.Instance.NowState = EnumGameState.Ready;
        }

        // 까마귀 이펙트
        EffectManager.Instance.SetPool(EffectKey, transform.position, EffectScale);

        // 애니메이션 & 이동 & 사운드 코루틴
        Action act = () => {
            anim.SetBool("Action", true);

            transform.DOMoveX(-12f, 8f);
            StartCoroutine(SoundCoroutine());            
        };

        IEnumerator delayCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(0.25f, act);
        StartCoroutine(delayCoroutine);   
    }

    public void Dead()
    {
        // 일단 하드코딩
        IsAlive = false;

        // DOKill, StopCoroutine
        transform.DOKill();
        StopAllCoroutines();

        // Effect
        EffectManager.Instance.SetPool("Dead", transform.position);

        IEnumerator coroutine;

        // 튜토리얼에만 적용
        if (Tutorial)
        {
            // 자막
            UIManager.Instance.SetNotice("붉은 빛은 큰 소리를 낸 까마귀를 삼켰다.", 4.0f);

            coroutine = DUSDJUtil.ActionAfterSecondCoroutine(0.5f, () => {
                // 조작 재개 & 플레이어 카메라
                GameManager.Instance.NowState = EnumGameState.Action;
                EffectManager.Instance.ZoomTarget(GameManager.Instance.PlayerChara.transform, 4.0f);
                gameObject.SetActive(false);
            });
        }
        // 평상시
        else
        {
            coroutine = DUSDJUtil.ActionAfterSecondCoroutine(0.5f, () => {                
                gameObject.SetActive(false);
            });
        }
        StartCoroutine(coroutine);

    }

    public void Damage(float value)
    {
        if (!IsAlive)
        {
            return;
        }

        HP -= value;
    }

    

    IEnumerator SoundCoroutine()
    {
        float t = SoundData.ActionCoolDown;

       
        while (true)
        {
            if (t >= SoundData.ActionCoolDown)
            {
                t -= SoundData.ActionCoolDown;

                AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
                
                if(SoundLoop == false)
                {
                    break;
                }
            }
            else
            {
                t += Time.deltaTime;
            }

            yield return null;
        }
    }
}
