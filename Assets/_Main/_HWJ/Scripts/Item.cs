using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { ThrowItem, Carriable, UnCarriable };
    public ItemType itemType;
    public bool isGet;
    public bool isUse;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        isGet = false;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void UseItem()
    {
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
