using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueLightArea : MonoBehaviour, ISaveLoad
{
    [Header("이 빛에 닿으면 붉은빛 없어짐")]public bool IsSpecial = false;

    public bool IsActivated = false;    
    public bool IsPassed = false;

    private CollisionTrigger BlueLightTrigger;
    private Transform BlueLightPoint;

    private Collider2D col;

    private void Awake()
    {
        ISaveLoadInit();

        col = GetComponent<Collider2D>();

        BlueLightTrigger = transform.Find("BlueLightPoint").GetComponent<CollisionTrigger>();
        BlueLightPoint = transform.Find("BlueLightPoint");
    }


    public void ChildTriggerEnter()
    {
        if (IsPassed)
        {
            return;
        }

        LightManager.Instance.SetBlueLight(BlueLightPoint.position);
        col.enabled = true;        
    }

    public void ChildTriggerExit()
    {
        if (IsPassed)
        {
            return;
        }

        if (IsActivated)
        {
            LightManager.Instance.CloseBlueLight();

            col.enabled = false;
            IsPassed = true;
            Debug.Log(string.Format("col.enabled : {0}", col.enabled));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsActivated = true;


            SaveManager.Instance.SaveAll();
        }
    }


    #region ISaveLoad

    public struct StructSaveData
    {
        public bool IsActivated;
        public bool IsPassed;
    }
    public StructSaveData SaveData;

    public void ISaveLoadInit()
    {
        SaveManager.Instance.AddSaveObject(this);
    }

    public void ISave()
    {
        SaveData.IsActivated = IsActivated;
        SaveData.IsPassed = IsPassed;
    }

    public void ILoad()
    {
        IsActivated = SaveData.IsActivated;
        IsPassed = SaveData.IsPassed;
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
