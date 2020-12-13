using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "GameManager";
                    instance = container.AddComponent(typeof(GameManager)) as GameManager;
                }
            }

            return instance;
        }
    }

    #endregion

    #region Camera (Cinemachine)

    [HideInInspector] public CinemachineVirtualCamera Cinemachine;
    [HideInInspector] public CinemachineConfiner Confiner;

    #endregion

    #region Player

    [HideInInspector] public Player PlayerChara;

    #endregion

    #region Game States

    public EnumGameState NowState;

    #endregion

    #region Scenario

    public Scenario NowScenario;

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

        // 모든 Init의 뿌리 깊은 나무
        Init();
    }

    public void Init()
    {
        // 카메라 캐싱
        Cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
        Confiner = Cinemachine.GetComponent<CinemachineConfiner>();

        // Player 찾기 & 시네머신 Follow 적용
        PlayerChara = FindObjectOfType<Player>();
        Cinemachine.Follow = PlayerChara.transform;

        AudioManager.Instance.Init();
        EffectManager.Instance.Init();
        LightManager.Instance.Init();

        NowState = EnumGameState.Ready;
    }

    public void ChangeState(EnumGameState state)
    {
        NowState = state;
    }
}
