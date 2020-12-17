using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : Item
{

    private bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        OnGround = false;
    }
    private void Update()
    {
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, groundLayer);

        if (OnGround)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            //gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
            isGet = false;
        }else
        {
            rb.gravityScale = 1;
            this.gameObject.transform.position = Vector2.zero;
        }
    }

    public override void UseItem()
    {
        StartCoroutine(DisableMovement(0.5f));
        rb.velocity = gameObject.transform.right * 10f;
        rb.gravityScale = 1;
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

        if (other.gameObject.CompareTag("Player"))
        {
            if (isActive && !isGet)
            {
            }
        }
    }
    IEnumerator DisableMovement(float time)
    {
        isActive = false;
        yield return new WaitForSeconds(time);
        isActive = true;
    }

}
