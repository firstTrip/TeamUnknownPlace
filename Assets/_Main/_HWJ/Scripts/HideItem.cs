using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideItem : Item
{


    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void UseItem()
    {
        spriteRenderer.sortingLayerName = "Middleground_AP";
        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
        this.gameObject.transform.position = transform.position;
        StartCoroutine(DisableUse(0.2f));
    }
    IEnumerator DisableUse( float time)
    {
        isUse = true;
        yield return new WaitForSeconds(time);
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
                        Debug.Log("isUse");
                        spriteRenderer.sortingLayerName = "Middleground_AP";
                        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
                        other.GetComponentInParent<Player>().isInvincibility = true;
                        isUse = true;
                        return;
                    }

                    other.GetComponentInParent<Player>().isInvincibility = true;
                    gameObject.transform.SetParent(other.transform.GetChild(0).GetChild(0));
                }
            }
        }
        else return;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().isInvincibility = false;
            isUse = false;

        }
    }
}
