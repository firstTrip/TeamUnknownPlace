using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ZoomAction : ObjectBase, ICallback

{
    public Transform FollowTarget;
    public float OrthoValue = 4.0f;
    public float Duration = 0;

    public bool StopWhileZoom = false;

    private bool isUsed = false;

    public override void Init()
    {
        base.Init();

        isUsed = false;

    }


    #region ICallback

    public void CallbackAction()
    {
        ObjectAction();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion

    public override void ObjectAction()
    {
        isUsed = true;

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

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(isUsed == true)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            ObjectAction();
        }
        
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    


}
