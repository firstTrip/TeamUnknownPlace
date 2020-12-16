using Cinemachine;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Scenario : MonoBehaviour
{
    
    private PlayableDirector TimeLine; // 타임라인 있는지?
    private Collider2D CinemachineCollider; // 시네머신 영역
    private GameObject WallCollider; // 못 돌아가게 막을 벽
    private LightArea[] LightAreaArray; // 이 씬에서 다룰 빛 영역

    // 체크
    private bool isEnter = false;
    // 1회용
    private bool isPlayed = false;

    private void Awake()
    {
        CinemachineCollider = transform.Find("CinemachineCollider").GetComponent<Collider2D>();
        TimeLine = transform.GetComponentInChildren<PlayableDirector>();        
        WallCollider = transform.Find("WallCollider").gameObject;
        LightAreaArray = transform.Find("LightArea").GetComponentsInChildren<LightArea>();

        ObjectsSetActive(false);
    }

    public void ObjectsSetActive(bool value)
    {
        CinemachineCollider?.gameObject.SetActive(value);
        TimeLine?.gameObject.SetActive(value);
        
        if (value)
        {
            WallCollider?.SetActive(value);
        }
        
        foreach (var item in LightAreaArray)
        {
            item.gameObject.SetActive(value);
        }
    }

    public void ScenarioSequence()
    {
        // 시나리오 오브젝트 활성화
        ObjectsSetActive(true);

        // GameManager Scenario Set & GameState Ready
        GameManager.Instance.NowScenario = this;
        GameManager.Instance.ChangeState(EnumGameState.Ready);

        // 시네머신 콜라이더 해제
        GameManager.Instance.Confiner.m_BoundingShape2D = null;

        // 타임라인 체크
        if (TimeLine != null)
        {
            TimeLine.Play(); // 이후 Event로 AfterTimeLine() 이용

        }
        // 타임라인 없으면 바로 다음
        else
        {
            AfterTimeLine();
        }
    }

    /// <summary>
    /// 타임라인 체크 이후 발동
    /// </summary>
    public void AfterTimeLine()
    {
        // 시네머신 콜라이더 설정
        GameManager.Instance.Confiner.m_BoundingShape2D = CinemachineCollider;

        // 시네머신 Follow 적용
        GameManager.Instance.Cinemachine.Follow = GameManager.Instance.PlayerChara.transform;

        // GameState Action
        GameManager.Instance.ChangeState(EnumGameState.Action);

        // isPlayed
        isPlayed = true;
    }


    /// <summary>
    /// 스테이지 진입을 의미함
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(string.Format("{0} : Enter On Scenario", collision.gameObject.name));

        if (collision.CompareTag("Player"))
        {
            if (isEnter)
            {
                return;
            }

            // 재상영 안함.
            if (isPlayed)
            {
                return;
            }

            isEnter = true;
            ScenarioSequence();
        }
    }


    /// <summary>
    /// 현재 디자인에서는 사실상 다음 스테이지 진입을 의미함
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isEnter)
            {
                return;
            }

            isEnter = false;

            // 스테이지 오브젝트 모두 false
            ObjectsSetActive(false);
        }
    }
}