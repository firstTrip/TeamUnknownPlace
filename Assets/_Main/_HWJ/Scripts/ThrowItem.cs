using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : Item
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UseItem()
    {

        rb.velocity = gameObject.transform.right * 10f;
        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
        isGet = false;

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log(isGet);
        if (!isGet)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.GetComponentInParent<PlayerTest>().GetItemStatus())
                {
                    isGet = true;
                    gameObject.transform.SetParent(other.transform.GetChild(0));

                }
            }

        }
        else return;


    }
}
