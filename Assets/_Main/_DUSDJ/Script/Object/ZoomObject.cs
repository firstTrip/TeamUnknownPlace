using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ZoomObject : ObjectBase, ICallback

{
    public Transform FollowTarget;
    public float OrthoValue = 4.0f;
    public float Duration = 0;

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
