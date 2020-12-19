using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EffectAction : ActionObject
{
    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;

    private Vector3 TargetPosition;
    private bool SetTarget = false;

    protected override void AfterCheckAction()
    {
        if (SetTarget)
        {
            EffectManager.Instance.SetPool(EffectKey, TargetPosition, EffectScale);
        }
        else
        {
            EffectManager.Instance.SetPool(EffectKey, transform.position, EffectScale);
        }

    }

    public void SetTargetPosition(Vector3 pos)
    {
        TargetPosition = pos;
        SetTarget = true;
    }
}

