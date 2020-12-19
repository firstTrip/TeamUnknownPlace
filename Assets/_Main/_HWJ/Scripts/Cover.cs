using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Item
{
    private SpriteRenderer spriteRenderer;
    private GameObject GO;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
            //UseMaterial();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetTrigger("TakeOff");
        }
    }

    public override void UseItem()
    {
        if (StarFlag)
        {
            this.gameObject.SetActive(true);
            GO.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            this.gameObject.transform.position = transform.position;
            StartCoroutine(DisableUse(0.2f));
        }
      
    }
    public override void UseMaterial()
    {
        base.UseMaterial();
    }
    IEnumerator DisableUse(float time)
    {
        isUse = true;
        yield return new WaitForSeconds(time);
        isUse = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rose"))
        {

            Debug.Log("Rose");
            GO.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            this.gameObject.SetActive(true);
            this.gameObject.transform.position = transform.position;
            StartCoroutine(DisableUse(0.2f));
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
                        other.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
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
            spriteRenderer.sortingLayerName = "Middleground_BP";
            isUse = false;
        }

       
    }


}
