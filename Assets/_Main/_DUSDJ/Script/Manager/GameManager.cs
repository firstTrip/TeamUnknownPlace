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

    #region GamePause

    private bool isGamePaused = false;

    public void PauseGame(bool trueIsPause)
    {
        isGamePaused = trueIsPause;

        if (trueIsPause)
        {
            ChangeState(EnumGameState.Ready);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1.0f;
            ChangeState(EnumGameState.Action);
        }
    }
    #endregion

    private void Update()
    {
        #region Game Pause

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused && NowState == EnumGameState.Ready)
            {
                UIManager.Instance.SetGamePause(false);
            }
            else if(!isGamePaused && NowState == EnumGameState.Action)
            {
                UIManager.Instance.SetGamePause(true);
            }

            return;
        }

        if (isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                UIManager.Instance.UpdatePauseMenu(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                UIManager.Instance.UpdatePauseMenu(1);
            }
            else if (Input.GetKeyDown(KeyCode.Z)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Space))
            {
                UIManager.Instance.PauseMenuAction();
            }
        }

        #endregion


        #region Cheat Key

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveManager.Instance.SaveAll();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.Instance.LoadAll();
        }

        #endregion

    }


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

        DebugManager.Instance.SetText(DebugManager.Instance.GameStateText, NowState.ToString());

        if (state.Equals(EnumGameState.Action))
        {
            PlayerChara.ForceCanMove(true);
        }
    }
}
