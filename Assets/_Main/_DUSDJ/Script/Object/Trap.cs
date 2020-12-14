using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : ObjectBase, ICallback
{
    [Header("함정 지연시간")] public float TrapDelay;
    [Header("함정 밟았을때 사운드")]public StructSoundData TrapActivateSound;
    private Vector3 TrapActivateSoundScale;

    private bool isPlayerInRange;
    private bool isActivated;

    private Animator anim;

    public override void Init()
    {
        base.Init();

        isPlayerInRange = false;
        isActivated = false;

        TrapActivateSoundScale = new Vector3(TrapActivateSound.SoundWaveScale, TrapActivateSound.SoundWaveScale, 1.0f);

        anim = GetComponentInChildren<Animator>();
    }
    
    public override void ObjectAction()
    {
        Debug.Log("Trap Action");
        isActivated = true;

        /* 덜걱 소리가 나고, 일정시간 뒤에 발동 */
        //Sound & SoundWave
        EffectManager.Instance.SetPool("SoundWave", transform.position, TrapActivateSoundScale);        
        AudioManager.Instance.PlaySound(TrapActivateSound.SoundKey, TrapActivateSound.SoundValue, transform.position);

        // 지연시간 후 액션 코루틴
        IEnumerator coroutine = DUSDJUtil.ActionAfterSecondCoroutine(TrapDelay, AfterAction);
        StartCoroutine(coroutine);        
    }

    private void AfterAction()
    {
        // Sound Data
        EffectManager.Instance.SetPool("SoundWave", transform.position, SoundWaveScale);
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position);

        // 함정 변화 처리 (애니메이션?)
        anim.SetBool("Action", true);

        // 아직도 영역 안에 있을 때
        if (isPlayerInRange)
        {
            Debug.Log("플레이어 죽어야함");
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (isActivated == false)
            {
                ObjectAction();
            }
            
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }


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
}
