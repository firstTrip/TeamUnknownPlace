using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionObject : MonoBehaviour, ISaveLoad
{
    private bool isUsed = false;

    protected virtual void Awake()
    {
        isUsed = false;

        ISaveLoadInit();
    }

    public virtual void ObjectAction()
    {
        if (isUsed)
        {
            return;
        }

        isUsed = true;
        AfterCheckAction();
    }

    protected abstract void AfterCheckAction();


    #region ISaveLoad
    public struct StructSaveData
    {
        public Vector3 Position;
        public bool IsUsed;
    }
    public StructSaveData BaseSaveData;


    public void ISaveLoadInit()
    {
        SaveManager.Instance.AddSaveObject(this);
    }

    public virtual void ISave()
    {
        BaseSaveData.Position = transform.position;
        BaseSaveData.IsUsed = isUsed;
       
    }

    public virtual void ILoad()
    {
        transform.position = BaseSaveData.Position;
        isUsed = BaseSaveData.IsUsed;
    }

    public void ISaveDelete()
    {
        SaveManager.Instance.DeleteSaveObject(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion

}