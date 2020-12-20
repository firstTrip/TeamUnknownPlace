using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(UIManager)) as UIManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "UIManager";
                    instance = container.AddComponent(typeof(UIManager)) as UIManager;
                }
            }

            return instance;
        }
    }

    #endregion


    #region UI Cashing

    public GameObject GamePauseObject;
    private Animator GamePauseAnim;

    public GameObject GameOverObject;

    public Color SelectedColor;
    public Color UnSelectedColor;

    public TextMeshProUGUI NoticeText;

    #endregion

    #region Game Over
    public Image[] GameOverMenuArray;
    public enum GameOverMenu
    {
        Restart = 0,
        Menu = 1,
    }
    private GameOverMenu selectedGameOverMenu = 0;

    private bool isGameOverAnimating = false;


    public void SetGameOver(bool trueFalse)
    {
        if (isGameOverAnimating)
        {
            return;
        }

        if (trueFalse)
        {
            isGameOverAnimating = true;
            GameOverObject.SetActive(true);
            // GameManager.Instance.PauseGame(true);
        }
        else
        {
            GameOverObject.SetActive(false);
            // GameManager.Instance.PauseGame(false);
        }
    }

    public void AfterGameOverAnimation()
    {
        isGameOverAnimating = false;
        selectedGameOverMenu = (GameOverMenu)1;
        UpdateGameOverMenu(-1);
    }

    public void UpdateGameOverMenu(int i)
    {
        if (isGameOverAnimating)
        {
            return;
        }

        if(i == -1)
        {
            if(selectedGameOverMenu == 0)
            {
                selectedGameOverMenu = (GameOverMenu)1;
            }
            else
            {
                selectedGameOverMenu -= 1;
            }
        }
        else
        {
            if (selectedGameOverMenu == (GameOverMenu)1)
            {
                selectedGameOverMenu = 0;
            }
            else
            {
                selectedGameOverMenu += 1;
            }
        }

        for (int k = 0; k < GameOverMenuArray.Length; k++)
        {
            if(k == (int)selectedGameOverMenu)
            {
                GameOverMenuArray[k].color = SelectedColor;
                continue;
            }

            GameOverMenuArray[k].color = UnSelectedColor;
        }
    }

    public void GameOverMenuAction()
    {
        if (isGameOverAnimating)
        {
            return;
        }

        switch (selectedGameOverMenu)
        {
            case GameOverMenu.Restart:
                GameManager.Instance.GameOver(false);
                SaveManager.Instance.LoadAll();
                break;


            case GameOverMenu.Menu:
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(0);
                break;
        }
    }

    #endregion

    #region Game Pause

    public Image[] MenuArray;
    public enum PauseMenu
    {
        Continue = 0,
        Menu = 1,
        Exit = 2,
    }
    private PauseMenu selectedPauseMenu = 0;
    private bool isPauseExiting = false;
    public void SetGamePause(bool trueFalse)
    {
        if (isPauseExiting)
        {
            return;
        }

        if (trueFalse)
        {
            selectedPauseMenu = (PauseMenu)1;
            UpdatePauseMenu(-1);

            GamePauseObject.SetActive(true);
            GameManager.Instance.PauseGame(true);
        }
        else
        {
            isPauseExiting = true;
            GamePauseAnim.SetTrigger("Exit");
        }        
    }

    public void ExitGamePause()
    {
        isPauseExiting = false;

        GamePauseObject.SetActive(false);
        GameManager.Instance.PauseGame(false);
    }

    public void UpdatePauseMenu(int i)
    {
        if (isPauseExiting)
        {
            return;
        }

        if(i == -1)
        {
            if (selectedPauseMenu == 0)
            {
                selectedPauseMenu = (PauseMenu)2;
            }
            else
            {
                selectedPauseMenu -= 1;
            }            
        }
        else
        {
            if (selectedPauseMenu == (PauseMenu)2)
            {
                selectedPauseMenu = 0;
            }
            else
            {
                selectedPauseMenu += 1;
            }
        }

        for (int k = 0; k < MenuArray.Length; k++)
        {
            if(k == (int)selectedPauseMenu)
            {
                MenuArray[k].color = SelectedColor;
                continue;
            }

            MenuArray[k].color = UnSelectedColor;
        }
    }

    public void PauseMenuAction()
    {
        if (isPauseExiting)
        {
            return;
        }

        switch (selectedPauseMenu)
        {
            case PauseMenu.Continue:
                SetGamePause(false);
                break;


            case PauseMenu.Menu:
                DOTween.KillAll();
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(0);
                break;


            case PauseMenu.Exit:
#if UNITY_EDITOR
                Debug.Log("Exit Game");
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                break;
        }
    }

    #endregion

    #region Tween

    Sequence sq;

    #endregion

    private void Awake()
    {
        GamePauseAnim = GamePauseObject.GetComponent<Animator>();

        if (NoticeText == null)
        {
            NoticeText = GameObject.Find("BottomCanvas/NoticeText").GetComponent<TextMeshProUGUI>();
        }

    }

    public float TextDelay = 0.3f;
    public IEnumerator NoticeCoroutine;

    public void SetNotice(string content, float duration = -1)
    {
        if(NoticeCoroutine != null)
        {
            StopCoroutine(NoticeCoroutine);
            NoticeCoroutine = null;
        }

        NoticeText.text = content;
        NoticeText.DOFade(0, 0);
        NoticeText.DOFade(1.0f, 2.0f).OnComplete(()=>
        {
            if(duration > 0)
            {
                NoticeCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(duration, CleanNotice);
                StartCoroutine(NoticeCoroutine);
            }
        });
    }

    public void CleanNotice()
    {
        NoticeText.DOFade(0, 0.3f);
    }


    public void ShowDeadUI(bool trueOrFalse)
    {

    }
}
