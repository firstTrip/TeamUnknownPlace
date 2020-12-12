using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;

    private bool canMove;
    private bool walk;
    private bool dash;

    private PlayerCollision coll;


    [SerializeField]private float JumpForce;
    [SerializeField] private float moveForce;
    [SerializeField] private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
        Move();

        if (coll.OnGround) 
            Jump();
    }

    private void init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision>();
        canMove = true;
        moveSpeed = 1f;
        moveForce = 1.5f;
        JumpForce = 8f;
    }

    private void InputManager()
    {
        walk = Input.GetKeyDown(KeyCode.LeftControl);
        dash = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void Move()
    {

        float h = Input.GetAxisRaw("Horizontal");

        if (!canMove)
            return;

        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (walk)
            moveForce = 0.5f;

        if (dash)
            moveForce = 3f;

        if(!walk && !dash)
        {
            if (rb.velocity.x > moveForce)
                rb.velocity = new Vector2(moveForce, rb.velocity.y);
            else if (rb.velocity.x < moveForce * (-1))
                rb.velocity = new Vector2(moveForce * (-1), rb.velocity.y);
        }
       
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            StartCoroutine(DisableMovement(1f));
        }
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

}
