﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Mob_Cat : MonoBehaviour
{
    public StructSoundData SoundData;

    [Header("도착지점")]public Transform ArrivalPoint;
    [Header("N번만에 도착함")] public int NumOfMovement = 4;
    [Header("도착 보정")]public float MinDistance = 0.05f;

    [Header("이동속도")] public float MoveSpeed;
    [Header("멈춰서는 시간")] public float MoveDealy = 0.25f;

    private int NumOfActivatedMovement = 0;

    private float stay = 0f;

    private Vector2 startingPoint;
    private Vector2 endingPoint;
    private Vector2 movePoint;
    private bool isArrive = false;
    private bool isActivated = false;




    private enum CatAnim { Walk , Touch }
    private CatAnim _catAnim;

    private string currentAnimation;

    // skeletonAnimation  에다가 HWJ//Prefab//CatAnim//CatAnimbation 넣으시면 됩니다
    // AnimClip 클립 사이즈 2 로 해주시고 첫번째에 walk 두번째에 touch 넣으시면됩니다
    // 클립위치는 Assets/_Main/_HWJ/Spine/Cat_2acts/ReferenceAssets 여기입니다.
    // 사용법 원하는 위치에

    // _catAnim = CatAnim. 원하는 애니메이션
    // SetCurrentAnimation(catAnim);
    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        startingPoint = transform.position;
        endingPoint = ArrivalPoint.transform.position;
        movePoint = transform.position;
    }

    private void Update()
    {
        if (!isActivated)
        {
            return;
        }

        Walk();
    }

    public void Load()
    {

    }

    public void ObjectAction()
    {
        if (isActivated)
        {
            return;
        }
        isActivated = true;
    }

    public void MakeSound()
    {
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
    }

    public void SetMovePoint()
    {

        if (NumOfActivatedMovement >= NumOfMovement)
        {
            return;
        }

        NumOfActivatedMovement += 1;

        isArrive = false;
        movePoint = Vector2.Lerp(startingPoint, endingPoint, (float)NumOfActivatedMovement / NumOfMovement);        
    }

    public void Walk()
    {
        if (isArrive)
        {
            if (stay >= MoveDealy)
            {
                if (NumOfActivatedMovement >= NumOfMovement)
                {
                    MakeSound();
                    RunAway();
                    return;
                }

                stay = 0;                
                SetMovePoint();
                MakeSound();
                Debug.LogWarning(NumOfActivatedMovement);
            }
            else
            {
                stay += Time.deltaTime;
                return;
            }    
            
            return;
        }

        float distance = Vector2.Distance((Vector2)transform.position, movePoint);
        if (distance <= MinDistance)
        {
            isArrive = true;
            return;
        }

        Vector2 direction = (movePoint - (Vector2)transform.position).normalized;
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    public void RunAway()
    {
        Debug.Log("고양이 Out");
        gameObject.SetActive(false);
    }


    #region 

    private void AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {

        if (animClip.name.Equals(currentAnimation))
            return;

        //해당 애님으로 변경
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        currentAnimation = animClip.name;

    }
    #endregion

    private void SetCurrentAnimation(CatAnim _catAnim)
    {

        switch (_catAnim)
        {
            // ( 애님 이름  , 루프 여부 , 재생시간)
            case CatAnim.Walk:
                AsncAnimation(AnimClip[(int)CatAnim.Walk], true, 1f);
                break;

            case CatAnim.Touch:
                AsncAnimation(AnimClip[(int)CatAnim.Touch], true, 0.5f);
                break;

        }

    }

}
