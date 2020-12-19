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
        StarFlag = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
            //UseMaterial();
    }
    private void Update()
    {
    }

    public override void UseItem()
    {
        /*
        if (StarFlag)
        {
            this.gameObject.SetActive(true);
            GO.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            this.gameObject.transform.position = transform.position;
            StartCoroutine(DisableUse(0.2f));
        }
        */
      
    }
    public override void UseMaterial()
    {
        if (StarFlag)
        {
            GameObject Go = Instantiate(this.gameObject, transform.position, Quaternion.identity);
            Go.transform.localScale = new Vector3(0.25f, 0.25f, 1);
            Go.GetComponent<SpriteRenderer>().material = _material;
            Go.GetComponent<Item>().enabled = false;
            Go.transform.SetParent(this.gameObject.transform);
            StarFlag = false;
        }
     
    }
    IEnumerator DisableUse(float time)
    {
        isUse = true;
        yield return new WaitForSecondsRealtime(time);
        isUse = false;
    }


    IEnumerator TakeOffTime(float time)
    {
        
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
            //spriteRenderer.sortingLayerName = "Middleground_BP";
            isUse = false;
        }

       
    }


}
