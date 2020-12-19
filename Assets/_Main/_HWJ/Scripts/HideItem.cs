using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideItem : Item
{
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        UseMaterial();
    }

    public override void UseItem()
    {
        spriteRenderer.sortingLayerName = "Middleground_AP";
        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
        this.gameObject.transform.position = transform.position;
        this.gameObject.transform.GetChild(0).SetParent(this.transform);
        this.gameObject.transform.GetChild(0).position = transform.position;
        StartCoroutine(DisableUse(0.2f));
    }

    public override void UseMaterial()
    {
        base.UseMaterial();
    }
    IEnumerator DisableUse( float time)
    {
        isUse = true;
        yield return new WaitForSecondsRealtime(time);
        isUse = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isUse)
        {
            if (other.gameObject.CompareTag("Player"))
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
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().isInvincibility = false;
            //spriteRenderer.sortingLayerName = "Middleground_BP";
            isUse = false;

        }
    }
}
