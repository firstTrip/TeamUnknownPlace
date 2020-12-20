using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideItem : Item, ISaveLoad
{
    private SpriteRenderer spriteRenderer;


    private IEnumerator disableCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        UseMaterial();

        ISaveLoadInit();
    }

    public override void UseItem()
    {
        spriteRenderer.sortingLayerName = "Middleground_AP";
        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
        this.gameObject.transform.position = transform.position;
        this.gameObject.transform.GetChild(0).SetParent(this.transform);
        this.gameObject.transform.GetChild(0).position = transform.position;
        
        if(disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);           
        }
        disableCoroutine = DisableUse(0.2f);
        StartCoroutine(disableCoroutine);
    }

    public override void UseMaterial()
    {
        base.UseMaterial();
    }

    IEnumerator DisableUse(float time)
    {
        isUse = true;
        yield return new WaitForSecondsRealtime(time);
        isUse = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isUse)
        {
            if (other.CompareTag("Player"))
            {
                if (other.GetComponentInParent<Player>().GetItemUse())
                {
                    if (itemType == ItemType.UnCarriable)
                    {
                        //spriteRenderer.sortingLayerName = "Middleground_AP";

                        other.GetComponentInParent<Player>().isInvincibility = true;
                        Debug.Log("isUse");
                        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
                        isUse = true;
                    }

                    if(itemType == ItemType.Carriable)
                    {
                        other.GetComponentInParent<Player>().isInvincibility = true;
                        gameObject.transform.SetParent(other.transform.GetChild(0).GetChild(0));
                    }
           
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().isInvincibility = false;
            //spriteRenderer.sortingLayerName = "Middleground_BP";
            isUse = false;

        }
    }

    #region ISaveLoad

    public struct StructSaveData
    {
        public Vector3 Position;
        public bool isUse;
    }
    public StructSaveData SaveData;

    public void ISaveLoadInit()
    {
        SaveManager.Instance.AddSaveObject(this);
    }

    public void ISave()
    {
        SaveData.Position = transform.position;
        SaveData.isUse = isUse;
    }

    public void ILoad()
    {
        transform.position = SaveData.Position;
        isUse = SaveData.isUse;
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
