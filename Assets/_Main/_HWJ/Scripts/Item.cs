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

    private SpriteRenderer spr;
    private GameObject outLineObject;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        isGet = false;
        isUse = false;
        StarFlag = true;
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        //collider2D = GetComponent<Collider2D>();        

        _material = Resources.Load<Material>("sprite_ingame_outlinecommon_2");
        UseMaterial();
    }

    public virtual void UseItem()
    {

    }

    

    public virtual void UseMaterial()
    {
        if (outLineObject != null)
        {
            return;
        }

        Debug.Log("into Material");
        outLineObject = new GameObject("OutLine");
        outLineObject.transform.SetParent(transform, false);
        outLineObject.transform.localPosition = Vector3.zero;
        outLineObject.transform.localRotation = Quaternion.identity;
        outLineObject.transform.localScale = new Vector3(1, 1, 1);

        SpriteRenderer outlineSpr = outLineObject.AddComponent<SpriteRenderer>();
        outlineSpr.sortingLayerID = spr.sortingLayerID;
        outlineSpr.sortingOrder = spr.sortingOrder - 1;
        outlineSpr.material = _material;
        outlineSpr.sprite = spr.sprite;

        StarFlag = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log(isGet);
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + BottomOffset, CollisionRadius);
    }
}
