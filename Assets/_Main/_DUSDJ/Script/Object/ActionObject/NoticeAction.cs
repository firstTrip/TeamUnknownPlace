using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeAction : MonoBehaviour, ICallback
{
    public float Duration;
    public string Content;

    public bool BroadCast = false;

    private bool isUsed = false;

    private void Awake()
    {
        isUsed = false;
    }

    private void ObjectAction()
    {
        UIManager.Instance.SetNotice(Content, Duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            isUsed = true;
            ObjectAction();

            if (BroadCast)
            {
                BroadcastMessage("OnTriggerEnter2D", collision);
            }
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