using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{


    [Header("Sound Data")] public StructSoundData SoundData;
    [Header("루프여부")] public bool SoundLoop = false;

    public GameObject DeletColl;
    public GameObject NextTaget;
    public float BellDuration;

    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 끝나는 시간")] public float EffectDuration;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ObjectAction()
    {
        StartRing(BellDuration);
    }

    public void ObjectAction2()
    {
        DeletColl.SetActive(false);
    }

    private IEnumerator disableCoroutine;
    private void StartRing(float time)
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = RingBell(time);
        StartCoroutine(disableCoroutine);
    }
    
    IEnumerator RingBell(float time)
    {
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
        EffectManager.Instance.SetPool(EffectKey, transform.position, EffectScale);
        yield return new WaitForSecondsRealtime(time);

        if(NextTaget != null)
            NextTaget.GetComponent<Bell>().ObjectAction();
    }
}
