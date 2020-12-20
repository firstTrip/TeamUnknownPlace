using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardobe : Item
{
    private SpriteRenderer spriteRenderer;
    public GameObject DoorSprite1;
    public GameObject DoorSprite2;
    public Sprite sprite;
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
        StartCoroutine(DisableUse(0.2f));
    }

    public override void UseMaterial()
    {
        base.UseMaterial();
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
                        DoorSprite1.SetActive(false);
                        DoorSprite2.SetActive(false);
                        spriteRenderer.sprite = sprite;
                        other.GetComponentInParent<Player>().isInvincibility = true;
                        Debug.Log("isUse");
                        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
                        isUse = true;
                    }

                    if (itemType == ItemType.Carriable)
                    {
                        other.GetComponentInParent<Player>().isInvincibility = true;
                        gameObject.transform.SetParent(other.transform.GetChild(0).GetChild(0));
                    }

                }
            }
        }

    }

    IEnumerator DisableUse(float time)
    {
        isUse = true;
        yield return new WaitForSecondsRealtime(time);
        isUse = false;
    }
}
