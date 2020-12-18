using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightMonster : MonoBehaviour
{
    [Header("소리1당 속도 비율")]public float MoveSpeed;
    [Header("데미지 = N-거리")] public float DamageBase = 2.0f;

    private GameObject previousTarget;
    private Vector2 TargetPoint;
    private bool isArrive = false;

    private float axcelSpeed = 0f;
    private float tracingTime = 0f;
    [Header("최대가속 시간")]public float AxcelMaxTime = 2.0f;

    [Header("B,R 시간")]public float BanishRevealTime = 2.0f;

    public bool IsHunting = false;

    #region Light

    private Light2D edgeLight;
    private float edgeIntensity;
    private float edgeInnerRadius;
    private float edgeOuterRadius;

    private Light2D coreLight;
    private float coreIntensity;
    private float coreInnerRadius;
    private float coreOuterRadius;

    private SpriteRenderer coreSpr;
    private float coreSprAlpha;

    private IEnumerator LightCoroutine;

    #endregion


    #region Sound

    // 소리 집중시간 코루틴
    private IEnumerator DurationCoroutine;

    private Sound nowSound;
    public Sound NowSound
    {
        get
        {
            return nowSound;
        }
        set
        {
            bool reset;
            // 새 소리의 출처가 이전 타겟과 같음 -> 속도 유지
            if (previousTarget != null
                    && value.SoundFrom != null
                    && value.SoundFrom.Equals(previousTarget))
            {
                reset = false;
            }
            // 없거나 다름 -> 속도 초기화
            else
            {
                reset = true;
            }

            // 1. 현재 집중하는 소리가 없음.
            if (nowSound == null)
            {
                SetSound(value, reset);
                return;
            }

            // 2. 집중하는 소리 있음
            // 2-1. 더 큰 소리를 들었을 때 -> 추적 변경
            if (nowSound.Value < value.Value)
            {
                SetSound(value, reset);
                return;
            }

            // 2-2. 동등한 소리
            if (nowSound.Value == value.Value)
            {
                // 2-2-0. 동등소리, 현 추적 출처가 불명 -> 추적 변경, 속도 변경                
                // 2-2-1. 동등소리, 다른 출처 -> 추적 변경 : value는 같음
                // 2-2-2. 동등소리, 같은 출처 -> 추적 유지 : value는 같음
                SetSound(value, reset);
            }

            // 그 외의 경우 -> 무시
            return;
        }
    }

    private void SetSound(Sound s, bool traceReset)
    {
        previousTarget = s.SoundFrom;

        nowSound = s;
        TargetPoint = nowSound.Position;
        isArrive = false;
        if (traceReset)
        {
            tracingTime = 0f;            
        }
        axcelSpeed = MoveSpeed * nowSound.Value;

        // 소리 지속시간 집중
        if (DurationCoroutine != null)
        {
            StopCoroutine(DurationCoroutine);
        }
        DurationCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(nowSound.Duration, () => {
            nowSound = null;
        });
        StartCoroutine(DurationCoroutine);

        // Debug
        if(previousTarget != null)
        {
            DebugManager.Instance.SetText(DebugManager.Instance.PrevTargetText, previousTarget.name);
        }
        else
        {
            DebugManager.Instance.SetText(DebugManager.Instance.PrevTargetText, "Null");
        }

        if(nowSound.SoundFrom != null)
        {
            DebugManager.Instance.SetText(DebugManager.Instance.SoundFromText, nowSound.SoundFrom.name);
        }
        else
        {
            DebugManager.Instance.SetText(DebugManager.Instance.SoundFromText, "Null");
        }
        
    }

    public void CleanSound()
    {
        previousTarget = null;
        nowSound = null;                
        tracingTime = 0f;
        isArrive = true;

        // 소리 지속시간 집중
        if (DurationCoroutine != null)
        {
            StopCoroutine(DurationCoroutine);
        }        
    }

    #endregion

    private int layerMask;
    private Player player;
    private bool IsActive = false;

    public List<IDamagable> DamageList;

    private void Awake()
    {
        // edge Light Origin
        edgeLight = GetComponent<Light2D>();
        edgeIntensity = edgeLight.intensity;
        edgeInnerRadius = edgeLight.pointLightInnerRadius;
        edgeOuterRadius = edgeLight.pointLightOuterRadius;

        // core Light Origin
        coreLight = GetComponentInChildren<Light2D>();
        coreIntensity = coreLight.intensity;
        coreInnerRadius = coreLight.pointLightInnerRadius;
        coreOuterRadius = coreLight.pointLightOuterRadius;

        // core Spr Origin
        coreSpr = GetComponentInChildren<SpriteRenderer>();
        coreSprAlpha = coreSpr.color.a;

        DamageList = new List<IDamagable>();

        tracingTime = 0f;
        stayTime = 0f;
    }

    public void Init()
    {
        player = GameManager.Instance.PlayerChara;
        layerMask = 1 << LayerMask.NameToLayer("BlueLight");

        IsActive = true;
        IsHunting = true;
    }

    private void Update()
    {
        if(nowSound == null)
        {
            DebugManager.Instance.SetText(DebugManager.Instance.NowSoundNullText, "Null");
        }
        else
        {
            DebugManager.Instance.SetText(DebugManager.Instance.NowSoundNullText, "Not Null");
        }

        if (!IsActive)
        {
            return;
        }

        DebugManager.Instance.SetText(DebugManager.Instance.IsHuntingBox, IsHunting.ToString());

        // Damage
        DamageAll();

        if (IsHunting)
        {
            Hunt();            
        }
        else
        {
            Gaze();
        }

        // Debug
        DebugManager.Instance.SetText(DebugManager.Instance.IsArriveText, isArrive.ToString());
        DebugManager.Instance.SetText(DebugManager.Instance.TracingTimeText, tracingTime.ToString());
        DebugManager.Instance.SetText(DebugManager.Instance.StayTimeText, string.Format("{0:f1}/{1:f1}",stayTime,StayLimit));
    }

    #region Hunt, Damage, DeadCheck, DeadLock

    [Header("목적지 보정")]public float ArrivalCheckValue = 0.03f;
    [Header("교착시간 한계")]public float StayLimit = 20f;
    private float stayTime = 0f;

    private void Hunt()
    {
        if (!isArrive)
        {
            // 이동을 하게 되면 일단 stay는 아닌거임
            stayTime = 0;

            // 1. TargetPoint 도달 체크
            float distance = Vector2.Distance(transform.position, TargetPoint);
            if(distance <= ArrivalCheckValue)
            {
                isArrive = true;
            }
            // 2. 목적지 도달하지 못했을 시
            else
            {
                tracingTime += Time.deltaTime;

                // 가속체크
                float axcel = Mathf.Lerp(0, axcelSpeed, tracingTime / AxcelMaxTime);

                // 이동
                Vector2 diretion = (TargetPoint - (Vector2)transform.position).normalized;
                transform.Translate(diretion * axcel * Time.deltaTime);
            }            
        }
        else
        {
            // 대기중
            stayTime += Time.deltaTime;
            // 교착상태 한계시간 종료, 강제탐색.
            if(stayTime >= StayLimit)
            {
                TargetPoint = GameManager.Instance.PlayerChara.transform.position;
            }
        }
    }
    
    public void DamageAll()
    {
        for (int i = 0; i < DamageList.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, DamageList[i].GetGameObject().transform.position);
            float dmg = DamageBase - distance;
            if(dmg < 0)
            {
                dmg = 0;
            }

            DamageList[i].Damage(dmg);
        }
    }

    public void DeadCheck(GameObject from)
    {
        if(NowSound.SoundFrom != null
            && NowSound.SoundFrom.Equals(from))
        {
            CleanSound();
        }
    }

    public void DeadLock()
    {
        CleanSound();

        stayTime = StayLimit;
    }

    #endregion

    #region Gaze, Bounce

    private void Gaze()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        DebugManager.Instance.SetText(DebugManager.Instance.DistanceBox, distance.ToString());

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, layerMask);
        Debug.DrawRay(transform.position, direction, Color.red, 0.1f);

        if (hit.collider != null)
        {
            DebugManager.Instance.SetText(DebugManager.Instance.IsHitBlueLightBox, hit.collider.gameObject.name);

        }
        else
        {
            DebugManager.Instance.SetText(DebugManager.Instance.IsHitBlueLightBox, "False");
            IsHunting = true;
        }
    }

    public void Bounce(Vector3 source, float power)
    {
        Debug.Log("Bounce!");

        IsActive = false;
        IsHunting = false;
        CleanSound();

        Vector3 direction = (transform.position - source).normalized;
        direction.z = 0;
        Vector3 endValue = transform.position + direction * power;

        Debug.DrawLine(transform.position, endValue, Color.white, 10f);

        transform.DOMove(endValue, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
            IsActive = true;
        });


    }

    public void BounceBySpecial(Vector3 source, float power)
    {
        Debug.Log("Bounce By Special!");

        IsActive = false;
        IsHunting = false;
        CleanSound();

        Vector3 direction = (transform.position - source).normalized;
        direction.z = 0;
        Vector3 endValue = transform.position + direction * power;

        transform.DOMove(endValue, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
            Banish();
        });
    }

    #endregion

    #region Disable

    public void Banish()
    {
        IsActive = false;
        IsHunting = false;
        CleanSound();

        if (LightCoroutine != null)
        {
            StopCoroutine(LightCoroutine);
        }
        LightCoroutine = BanishRevealCoroutine(false);
        StartCoroutine(LightCoroutine);
    }

    public void Reveal()
    {
        IsActive = true;

        if(LightCoroutine != null)
        {
            StopCoroutine(LightCoroutine);
        }
        LightCoroutine = BanishRevealCoroutine(true);
        StartCoroutine(LightCoroutine);

    }
    
    

    private IEnumerator BanishRevealCoroutine(bool trueIsReveal)
    {
        float t = 0;

        #region Set Reveal or Banish Values

        float nowEdgeIntensity = edgeLight.intensity;
        float nextEdgeIntensity;
        float nowEdgeInnerRadius = edgeLight.pointLightInnerRadius;
        float nextEdgeInnerRadius;
        float nowEdgeOuterRadius = edgeLight.pointLightOuterRadius;
        float nextEdgeOuterRadius;

        float nowCoreIntensity = coreLight.intensity;
        float nextCoreIntensity;
        float nowCoreInnerRadius = coreLight.pointLightInnerRadius;
        float nextCoreInnerRadius;
        float nowCoreOuterRadius = coreLight.pointLightOuterRadius;
        float nextCoreOuterRadius;

        Color nowColor = coreSpr.color;
        Color nextColor;

        // Reveal
        if (trueIsReveal)
        {
            // edge
            nextEdgeIntensity = edgeIntensity;
            nextEdgeInnerRadius = edgeInnerRadius;
            nextEdgeOuterRadius = edgeOuterRadius;

            // core
            nextCoreIntensity = coreIntensity;
            nextCoreInnerRadius = coreInnerRadius;
            nextCoreOuterRadius = coreOuterRadius;

            // core spr
            nextColor = new Color(nowColor.r, nowColor.g, nowColor.b, coreSprAlpha);
        }
        else
        {
            // edge
            nextEdgeIntensity = 0;
            nextEdgeInnerRadius = 0;
            nextEdgeOuterRadius = 0;

            // core
            nextCoreIntensity = 0;
            nextCoreInnerRadius = 0;
            nextCoreOuterRadius = 0;

            nextColor = new Color(nowColor.r, nowColor.g, nowColor.b, 0);
        }

        #endregion

        while (t < BanishRevealTime)
        {
            t += Time.deltaTime;

            float lerpTime = t / BanishRevealTime;

            // edge
            edgeLight.intensity = Mathf.Lerp(nowEdgeIntensity, nextEdgeIntensity, lerpTime);
            edgeLight.pointLightInnerRadius = Mathf.Lerp(nowEdgeInnerRadius, nextEdgeInnerRadius, lerpTime);
            edgeLight.pointLightOuterRadius = Mathf.Lerp(nowEdgeOuterRadius, nextEdgeOuterRadius, lerpTime);

            // core
            coreLight.intensity = Mathf.Lerp(nowCoreIntensity, nextCoreIntensity, lerpTime);
            coreLight.pointLightInnerRadius = Mathf.Lerp(nowCoreInnerRadius, nextCoreInnerRadius, lerpTime);
            coreLight.pointLightOuterRadius = Mathf.Lerp(nowCoreOuterRadius, nextCoreOuterRadius, lerpTime);

            // core spr
            coreSpr.color = Color.Lerp(nowColor, nextColor, lerpTime);

            yield return null;
        }

        /* Set Complete */
        // edge
        edgeLight.intensity = nextEdgeIntensity;
        edgeLight.pointLightInnerRadius = nextEdgeInnerRadius;
        edgeLight.pointLightOuterRadius = nextEdgeOuterRadius;

        // core
        coreLight.intensity = nextCoreIntensity;
        coreLight.pointLightInnerRadius = nextCoreInnerRadius;
        coreLight.pointLightOuterRadius = nextCoreOuterRadius;

        // core spr
        coreSpr.color = nextColor;
    }


    #endregion

    #region OnTrigger

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 활동정지시 (순서 아직 고려중)
        if (IsActive == false)
        {
            return;
        }

        // 파랑빛 충돌시 바운스
        if (collision.gameObject.layer == LayerMask.NameToLayer("BlueLight"))
        {
            // Special 체크
            if (collision.GetComponent<BlueLightArea>().IsSpecial)
            {
                BounceBySpecial(collision.transform.position, 4.0f);
            }
            else
            {
                Bounce(collision.transform.position, 2.0f);
            }

            return;
        }

        // 캐릭터 충돌시 천천히 침식
        if (collision.CompareTag("Player") || collision.CompareTag("Mob"))
        {
            IDamagable id = collision.GetComponent<IDamagable>();
            if (!DamageList.Contains(id))
            {
                Debug.Log("DamageList + " + id.GetGameObject().name);
                DamageList.Add(id);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 캐릭터 아웃시 침식해제
        if (collision.CompareTag("Player") || collision.CompareTag("Mob"))
        {
            IDamagable id = collision.GetComponent<IDamagable>();
            if (DamageList.Contains(id))
            {
                DamageList.Remove(id);
            }
        }
    }

    #endregion


}
