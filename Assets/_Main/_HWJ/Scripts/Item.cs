using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { ThrowItem, Carriable, UnCarriable };
    public ItemType itemType;
    public bool isGet;
    public bool isUse;
    public bool StarFlag;
    public Rigidbody2D rb;
    //public Collider2D collider2D;
    public float CollisionRadius = 0.15f;
    public LayerMask groundLayer;
    public bool OnGround;
    public Vector2 BottomOffset;
    public Material _material;
    // Start is called before the first frame update
    void Start()
    {
        isGet = false;
        isUse = false;
        StarFlag = true;
        rb = GetComponent<Rigidbody2D>();
        //collider2D = GetComponent<Collider2D>();

       
    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void UseItem()
    {

    }


    public virtual void UseMaterial()
    {
        
        
            Debug.Log("into Material");
            GameObject Go = Instantiate(this.gameObject, transform.position, Quaternion.identity);
            Go.transform.localScale = new Vector3(1, 1, 1);
            Go.GetComponent<SpriteRenderer>().material = _material;
            Go.GetComponent<Item>().enabled = false;
            Go.transform.SetParent(this.gameObject.transform);
            StarFlag = false;
        

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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + BottomOffset, CollisionRadius);
    }
}
