using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeAction : ActionObject
{
    public float Duration;
    public string Content;

    protected override void AfterCheckAction()
    {
        UIManager.Instance.SetNotice(Content, Duration);
    }

}