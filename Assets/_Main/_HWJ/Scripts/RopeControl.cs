using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeControl : MonoBehaviour
{

    private GameObject Go;
    public Rigidbody2D rb;
    private float x;
    public float PushForce;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        PushForce = 3;
        x = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (x != 0)
        {
            Debug.Log("asdqwezxc");
            rb.velocity = new Vector2(x * PushForce, rb.velocity.y);
        }
       
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           Go = collision.GetComponent<Player>().GetGameObject();
           x = Go.GetComponent<Player>().xRaw;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            x = 0;
            rb.velocity = new Vector2(x, rb.velocity.y);
        }
    }
}
