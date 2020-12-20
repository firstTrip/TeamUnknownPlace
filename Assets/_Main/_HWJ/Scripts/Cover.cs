using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Item
{
    private SpriteRenderer spriteRenderer;
    private GameObject GO;
    private Animator anim;
    // Start is called before the first frame update


    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UseMaterial();
    }


    public override void UseItem()
    {

    }

    public override void UseMaterial()
    {
        base.UseMaterial();
    }


    IEnumerator TakeOffTime(float time)
    {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(time);
        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
        this.gameObject.transform.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rose"))
        {
            Debug.Log("Rose");
            anim.SetTrigger("TakeOff");
            //GO.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            StartCoroutine(TakeOffTime(2f));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isUse)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.GetComponentInParent<Player>().GetItemUse())
                {
                    if (itemType == ItemType.Carriable)
                    {
                        this.gameObject.SetActive(false);
                        GO = other.gameObject;
                        other.GetComponentInParent<Player>().isInvincibility = true;
                        other.transform.GetChild(1).gameObject.SetActive(true);
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
            isUse = false;
        }

       
    }


}
