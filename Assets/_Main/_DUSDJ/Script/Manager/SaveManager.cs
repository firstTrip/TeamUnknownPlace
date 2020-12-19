using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(SaveManager)) as SaveManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "SaveManager";
                    instance = container.AddComponent(typeof(SaveManager)) as SaveManager;
                }
            }

            return instance;
        }
    }

    #endregion

    public Dictionary<GameObject, ISaveLoad> SaveObjectDic;

    private void Awake()
    {
        SaveObjectDic = new Dictionary<GameObject, ISaveLoad>();
    }

    public void AddSaveObject(ISaveLoad so)
    {
        if (SaveObjectDic.ContainsKey(so.GetGameObject()))
        {
            return;
        }

        SaveObjectDic.Add(so.GetGameObject(), so);
    }

    public void DeleteSaveObject(ISaveLoad so)
    {
        if (SaveObjectDic.ContainsKey(so.GetGameObject()))
        {
            return;
        }

        SaveObjectDic.Remove(so.GetGameObject());
    }

    public void SaveAll()
    {
        foreach (var so in SaveObjectDic)
        {
            so.Value.ISave();
        }
    }

    public void LoadAll()
    {
        EffectManager.Instance.Clean();
        AudioManager.Instance.Clean();

        foreach (var so in SaveObjectDic)
        {
            so.Value.ILoad();
        }
    }


}
