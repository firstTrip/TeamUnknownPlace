using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : Item
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void UseItem()
    {
        rb.velocity = gameObject.transform.right * 10f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        //isGet = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGet)
        { 
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.GetComponentInParent<Player>().GetItemStatus())
                {
                    isGet = true;
                    gameObject.transform.SetParent(other.transform.GetChild(0));
                }
            }

        }
        else return;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("asdqwezxc");
           gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
            rb.bodyType = RigidbodyType2D.Kinematic;
            
        }
    }
}
