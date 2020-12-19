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
    [HideInInspector] public CinemachineVirtualCamera PlayerCamera;
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


    #region Temp UI
    public GameObject TempUI;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TempUI.SetActive(!TempUI.activeSelf);
        }


        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveManager.Instance.SaveAll();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.Instance.LoadAll();
        }
    }


    #endregion


    public void Init()
    {
        // 카메라 캐싱
        Cinemachine = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        PlayerCamera = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();

        //Cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
        Confiner = Cinemachine.GetComponent<CinemachineConfiner>();

        // Player 찾기
        PlayerChara = FindObjectOfType<Player>();
        PlayerCamera.Follow = PlayerChara.transform;

        AudioManager.Instance.Init();
        EffectManager.Instance.Init();
        LightManager.Instance.Init();

        

        NowState = EnumGameState.Ready;
    }

    public void ChangeState(EnumGameState state)
    {
        NowState = state;

        if (state.Equals(EnumGameState.Action))
        {
            PlayerChara.ForceCanMove(true);
        }
    }
}
