using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Wardobe_New: MonoBehaviour, ISaveLoad
{
    private bool playerEnter = false;


    #region ISaveLoad
    public struct StructSaveData
    {
        public bool playerEnter;
    }
    public StructSaveData SaveData;
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void ILoad()
    {
        playerEnter = SaveData.playerEnter;
    }

    public void ISave()
    {
        SaveData.playerEnter = playerEnter;
    }

    public void ISaveDelete()
    {
        SaveManager.Instance.DeleteSaveObject(this);
    }

    public void ISaveLoadInit()
    {
        SaveManager.Instance.AddSaveObject(this);
    }
    #endregion

    private void Awake()
    {
        playerEnter = false;
        ISaveLoadInit();
    }

    private void Update()
    {
        if (!playerEnter)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Ending();
        }
    }

    public void Ending()
    {
        
        GameManager.Instance.Ending();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerEnter = false;
        }
    }
}
