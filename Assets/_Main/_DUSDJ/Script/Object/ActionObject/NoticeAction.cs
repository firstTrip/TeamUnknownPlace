using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeAction : MonoBehaviour, ICallback
{
    public float Duration;
    public string Content;

    private bool isUsed = false;

    private void Awake()
    {
        isUsed = false;
    }

    private void ObjectAction()
    {
        if (isUsed)
        {
            return;
        }
        isUsed = true;

        UIManager.Instance.SetNotice(Content, Duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ObjectAction();
        }
    }


    #region ICallback

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void CallbackAction()
    {
        ObjectAction();
    }

    #endregion

}