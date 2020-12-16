using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_Crow : MonoBehaviour, ICallback
{
    [System.Serializable]
    public struct StructSoundData
    {
        [Header("사운드 키")] public string SoundKey;
        [Header("사운드웨이브 크기")] public float SoundWaveScale;
        [Header("반복하는지?")] public bool Loop;
        [Header("소리발생 쿨다운")] public float ActionCoolDown;

        [Header("소리 밸류")] public int SoundValue;
    }

    [Header("Sound Data")] public StructSoundData SoundData;

    [Space]

    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 끝나는 시간")] public float EffectDuration;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;

    [Space]

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

    private Animator anim;


    #region ICallback

    public void CallbackAction()
    {
        Action();
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
    }

    public void Action()
    {
        LightManager.Instance.CloseBlueLight();
        LightManager.Instance.MainLight.IsHunting = true;

        EffectManager.Instance.SetPool(EffectKey, transform.position, EffectScale);

        Action act = () => {
            anim.SetBool("Action", true);

            transform.DOMoveX(-12f, 6f);
            StartCoroutine(SoundCoroutine());

            EffectManager.Instance.ZoomTarget(transform, 3.0f);

            // 캐릭터 정지까지 넣으면 좋을듯
        };


        IEnumerator delayCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(0.5f, act);
        StartCoroutine(delayCoroutine);   
    }

    public void Dead()
    {
        transform.DOKill();

        gameObject.SetActive(false);
    }

    public void Damage(float value)
    {
        hp -= value;
    }


    IEnumerator SoundCoroutine()
    {
        float t = SoundData.ActionCoolDown;

        Vector3 soundWaveScale = new Vector3(SoundData.SoundWaveScale, SoundData.SoundWaveScale, 1.0f);

        while (true)
        {
            if (t >= SoundData.ActionCoolDown)
            {
                t -= SoundData.ActionCoolDown;

                AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position);
                EffectManager.Instance.SetPool("SoundWave", transform.position, soundWaveScale);

                if(SoundData.Loop == false)
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
