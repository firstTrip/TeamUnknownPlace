using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public TextMeshProUGUI NoticeText;

    #endregion

    #region Tween

    Sequence sq;

    #endregion

    private void Awake()
    {
        if(NoticeText == null)
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

}
