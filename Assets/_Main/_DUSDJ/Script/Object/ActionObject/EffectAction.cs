using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EffectAction : MonoBehaviour
{
    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;

    private Vector3 TargetPosition;
    private bool SetTarget = false;

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

