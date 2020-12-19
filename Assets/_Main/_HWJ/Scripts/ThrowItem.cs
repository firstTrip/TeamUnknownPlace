using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : Item
{

    public float xThrow;
    public float yThrow;

    private GameObject GO;
    private bool isActive;
    private float x;


    protected override void Start()
    {
        base.Start();

        isActive = true;
        OnGround = false;

        UseMaterial();        
    }


    private void Update()
    {
        if (GO != null)
        {
            gameObject.transform.position = GO.transform.GetChild(0).gameObject.transform.GetChild(0).position;
            x = GO.GetComponent<Player>().transform.localScale.x;

        }

        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, groundLayer);


        if (OnGround)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);

            /*
            if (!isActive)
            {
                GameObject Go = Instantiate(this.gameObject, gameObject.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            */
        }else
        {
            if (!isActive)
            {
                rb.gravityScale = 1;

            }
          
        }
    }

    public override void UseItem()
    {
        GO = null;
        isActive = false;
        StartCoroutine(DisableMovement(0.5f));
        rb.velocity = new Vector2( x * xThrow * (-1) , yThrow);
    }

    public override void UseMaterial()
    {
        base.UseMaterial();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGet)
        { 
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.GetComponentInParent<Player>().GetItemStatus())
                {
                    GO = other.gameObject;
                    isGet = true;
                    gameObject.transform.SetParent(other.transform.GetChild(0).transform.GetChild(0).transform);
                    Debug.Log(other.transform.GetChild(0).transform.GetChild(0).transform);
                    
                    isActive = true;
                }
                
            }
            
        }
        else return;

    }

    IEnumerator DisableMovement(float time)
    {
        isGet = true;
        yield return new WaitForSeconds(time);
        isGet = false;
    }

}
