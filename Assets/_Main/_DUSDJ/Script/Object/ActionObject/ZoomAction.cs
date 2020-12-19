using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ZoomAction : ActionObject
{
    public Transform FollowTarget;
    public float OrthoValue = 4.0f;
    public float Duration = 0;

    public bool StopWhileZoom = false;

    protected override void AfterCheckAction()
    {
        
        EffectManager.Instance.ZoomTarget(FollowTarget, OrthoValue, Duration);

        if (StopWhileZoom)
        {
            GameManager.Instance.NowState = EnumGameState.Ready;
        }

        IEnumerator ZoomCoroutine = DUSDJUtil.ActionAfterSecondCoroutine(Duration, () =>
        {
            GameManager.Instance.NowState = EnumGameState.Action;
        });

        StartCoroutine(ZoomCoroutine);
    }

}
