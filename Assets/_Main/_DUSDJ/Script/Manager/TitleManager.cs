using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public Animator TitleAnim;

    public GameObject[] MenuArray;

    public enum TitleMenu
    {
        Start = 0,
        Continue = 1,
        Credit = 2,
        Exit= 3,
    }
    private TitleMenu selectedMenu = TitleMenu.Start;

    private bool isAnimating = false;
    private bool isTitleReady = false;

    private void Update()
    {
        if(!isTitleReady)
        {
            return;
        }

        if (isAnimating)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Z)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.Space))
        {
            isAnimating = true;
            TitleAnim.SetTrigger(selectedMenu.ToString());

            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(selectedMenu == 0)
            {
                selectedMenu = (TitleMenu)3;
            }
            else
            {
                selectedMenu -= 1;
            }

            SelectedUpdate();
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedMenu == (TitleMenu)3)
            {
                selectedMenu = 0;
            }
            else
            {
                selectedMenu += 1;
            }

            SelectedUpdate();
            return;
        }
    }

    public void SelectedUpdate()
    {
        for (int i = 0; i < MenuArray.Length; i++)
        {
            if(i == (int)selectedMenu)
            {
                MenuArray[i].SetActive(true);
                continue;
            }
            MenuArray[i].SetActive(false);
        }
    }

    #region Menu Actions

    public void TitleReady()
    {
        isTitleReady = true;
    }

    public void StartMenuAction()
    {
        SceneManager.LoadScene(2);
    }

    public void ContinueMenuAction()
    {
        SceneManager.LoadScene(2);
    }

    public void CreditMenuAction()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitMenuAction()
    {
#if UNITY_EDITOR
        Debug.Log("Exit Game");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}
