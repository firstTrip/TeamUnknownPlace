using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrap : MonoBehaviour
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

    [Header("사운드 끝나면 발동할 ICallback")]
    public GameObject Callback;
    private ICallback myCallback;


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

        if(Callback != null)
        {
            myCallback = Callback.GetComponent<ICallback>();
        }
        
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
        if (!SoundData.Loop && isUsed)
        {
            return;
        }

        if (ActionCoolDownNow > 0)
        {
            ActionCoolDownNow -= Time.deltaTime;
        }
        else
        {
            isUsed = true;
            ActionCoolDownNow = SoundData.ActionCoolDown;

            Sound s = AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);

            // 콜백이 있으면, 소리 끝난 다음에 발동
            if (Callback != null)
            {
                IEnumerator coroutine = DUSDJUtil.ActionAfterSecondCoroutine(s.Duration, myCallback.CallbackAction);
                StartCoroutine(coroutine);
            }
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



}
