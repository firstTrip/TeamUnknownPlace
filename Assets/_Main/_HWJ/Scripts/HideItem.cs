﻿using System.Collections;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UseItem()
    {
        if(itemType == ItemType.Carriable)
        {
            spriteRenderer.sortingLayerName = "Middleground_AP";
            this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
            this.gameObject.transform.position = transform.position;
            isGet = false;

        }


    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGet)
        {
            if (other.gameObject.CompareTag("Player"))
            {

                if (other.GetComponentInParent<Player>().GetItemStatus())
                {
                    if (itemType == ItemType.UnCarriable)
                    {
                        spriteRenderer.sortingLayerName = "Middleground_AP";
                        this.gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
                        //this.gameObject.transform.position = transform.position;
                        isGet = true;
                        return;
                    }
                    isGet = true;
                    gameObject.transform.SetParent(other.transform.GetChild(0));

                }
            }

        }
        else return;


    }

}